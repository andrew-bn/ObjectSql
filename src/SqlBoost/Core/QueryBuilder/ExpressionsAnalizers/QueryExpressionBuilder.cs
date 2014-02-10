﻿using SqlBoost.Core.Bo.CommandPreparatorDescriptor;
using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core.Misc;
using SqlBoost.Core.QueryBuilder.LambdaBuilder;
using SqlBoost.Core.SchemaManager;
using SqlBoost.Exceptions;
using SqlBoost.QueryInterfaces;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlBoost.Core.QueryBuilder.ExpressionsAnalizers
{
	internal class QueryExpressionBuilder : ExpressionVisitor, ISqlQueryBuilder
	{
		private bool _valueWasNull;
		protected IEntitySchemaManager SchemaManager { get; private set; }
		protected bool UseAliases { get; private set; }
		protected IStorageFieldType DbTypeInContext { get; set; }
		protected ICommandPreparatorsHolder CommandPreparatorsHolder { get; private set; }
		protected IDelegatesBuilder DelegatesBuilder { get; private set; }
		protected ISqlWriter SqlWriter { get; private set; }
		protected CommandText Text { get; set; }
		public QueryExpressionBuilder(IEntitySchemaManager schemaManager,
			IDelegatesBuilder expressionBuilder, ISqlWriter sqlWriter)
		{
			DbTypeInContext = null;
			SchemaManager = schemaManager;
			DelegatesBuilder = expressionBuilder;
			SqlWriter = sqlWriter;
		}
		public virtual string BuildSql(ICommandPreparatorsHolder commandPreparators, Expression expression, bool useAliases)
		{
			_valueWasNull = false;
			UseAliases = useAliases;
			CommandPreparatorsHolder = commandPreparators;
			return BuildSql(expression);
		}
		private string BuildSql(Expression expression)
		{
			Text = new CommandText();
			Visit(expression);
			return Text.ToString();
		}
		private void AddParameter(Expression accessor)
		{
			if (accessor.NodeType == ExpressionType.Constant &&
				((ConstantExpression)accessor).Value == null)
			{
				_valueWasNull = true;
				SqlWriter.WriteNull(Text);
			}
			else
			{
				var param = GetParameterDescriptor(accessor, DbTypeInContext);
				SqlWriter.WriteParameter(Text, param.Name);
			}
		}
		
		protected override Expression VisitConstant(ConstantExpression node)
		{
			AddParameter(node);
			return node;
		}
		protected override Expression VisitParameter(ParameterExpression node)
		{
			SqlWriter.WriteName(Text, node.Name);
			return node;
		}
		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression.NodeType == ExpressionType.Parameter)
			{
				var entitySchema = SchemaManager.GetSchema(node.Member.DeclaringType);

				if (UseAliases)
				{
					SqlWriter.WriteName(Text, ((ParameterExpression)node.Expression).Name);
					SqlWriter.WriteNameResolve(Text);
				}

				var storageField = entitySchema.GetStorageField(node.Member.Name);
				DbTypeInContext = storageField.DbType;
				SqlWriter.WriteName(Text, storageField.Name);
			}
			else
			{
				AddParameter(node);
			}
			return node;
		}
		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.NodeType == ExpressionType.Not)
			{
				var buf = Text;
				var not = SqlWriter.WriteNot(new CommandText(), BuildSql(node.Operand));
				SqlWriter.WriteBlock(buf, not.ToString());
				Text = buf;
				return node;
			}
			return base.VisitUnary(node);
		}
		protected override Expression VisitBinary(BinaryExpression node)
		{
			DbTypeInContext = null;
			
			var sql = Text;
			
			var left = BuildSql(node.Left);
			_valueWasNull = false;
			var right = BuildSql(node.Right);
			var commandText = new CommandText();
			switch (node.NodeType)
			{
				case ExpressionType.Equal:
					if (_valueWasNull)
						SqlWriter.WriteEqualNull(commandText, left);
					else
						SqlWriter.WriteEqual(commandText, left, right);
					 break;
				case ExpressionType.NotEqual:
					if (_valueWasNull)
						SqlWriter.WriteNotEqualNull(commandText, left);
					else
						SqlWriter.WriteNotEqual(commandText, left, right);
					 break;
				case ExpressionType.GreaterThan:
					SqlWriter.WriteGreater(commandText, left, right);
					break;
				case ExpressionType.GreaterThanOrEqual:
					SqlWriter.WriteGreaterOrEqual(commandText, left, right);
					break;
				case ExpressionType.LessThan:
					SqlWriter.WriteLess(commandText, left, right);
					break;
				case ExpressionType.LessThanOrEqual:
					SqlWriter.WriteLessOrEqual(commandText, left, right);
					break;
				case ExpressionType.AndAlso:
					SqlWriter.WriteAnd(commandText, left, right);
					break;
				case ExpressionType.OrElse:
					SqlWriter.WriteOr(commandText, left, right);
					break;
				case ExpressionType.Add:
					SqlWriter.WriteAdd(commandText, left, right);
					break;
				case ExpressionType.Subtract:
					SqlWriter.WriteSubtract(commandText, left, right);
					break;
				case ExpressionType.Divide:
					SqlWriter.WriteDivide(commandText, left, right);
					break;
				case ExpressionType.Multiply:
					SqlWriter.WriteMultiply(commandText, left, right);
					break;
			}
			Text = SqlWriter.WriteBlock(sql, commandText.ToString());
			return node;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Arguments.Count >= 1 && node.Arguments[0].Type == typeof(ITargetDatabase))
			{
				var buff = Text;

				var parts = new string[node.Arguments.Count-1];
				for (int i = 1; i < node.Arguments.Count; i++)
				{
					DbTypeInContext = null;
					Text = new CommandText();
					Visit(node.Arguments[i]);
					parts[i-1] = Text.ToString();
				}
				Text = buff;
				var meth = node.Method.DeclaringType.GetMethod("Render" + node.Method.Name, BindingFlags.Static | BindingFlags.IgnoreCase | BindingFlags.NonPublic);
				var renderResult = meth.Invoke(null,new object[]{ CommandPreparatorsHolder, parts});
				Text.Append(renderResult.ToString());
			}
			else throw new SqlBoostException("Invalid method call expression detected. Do not use method calls to pass parameters to SQL");

			return node;
		}

		private SingleParameterPreparator GetParameterDescriptor(Expression accessor, IStorageFieldType dbTypeInContext)
		{
			var descriptor = CommandPreparatorsHolder.Preparators
											 .Where(p=>p.PreparatorType == CommandPreparatorType.DatabaseCommandConstant ||
													   p.PreparatorType == CommandPreparatorType.DatabaseCommandParameter)
											 .Select(p=>p.AsSingleParameter())
											 .SingleOrDefault(d => ExpressionComparer.AreEqual(d.ValueAccessorExp, accessor) && Equals(d.DbType,dbTypeInContext));

			if (descriptor == null)
			{
				var parameterName = "p" + CommandPreparatorsHolder.Preparators.Count;
				var initializer = CreateParameterInitializer(parameterName, accessor, dbTypeInContext);

				descriptor =  IsConstant(accessor)
								? new DatabaseCommandConstantPreparator(parameterName,dbTypeInContext,accessor, initializer)
								: (SingleParameterPreparator)new DatabaseCommandParameterPreparator(parameterName,dbTypeInContext,accessor, initializer);
				CommandPreparatorsHolder.AddPreparator(descriptor);
			}

			if (descriptor.RootDemanding)
			{
				descriptor.AsDatabaseParameter().ParameterWasEncountered(CommandPreparatorsHolder.ParametersEncountered);
				CommandPreparatorsHolder.ParametersEncountered++;
			}

			return descriptor;
		}

		protected Action<IDbCommand, object> CreateParameterInitializer(string name, Expression accessor, IStorageFieldType dbTypeInContext)
		{
			return DelegatesBuilder.CreateDatabaseParameterFactoryAction(Expression.Constant(name, typeof(string)), accessor, dbTypeInContext);
		}

		private static bool IsConstant(Expression accessor)
		{
			var expressions = ExpressionEnumerator.Enumerate(accessor).ToArray();
			var isConstant = expressions.Length == 1 && (expressions[0] is ConstantExpression);
			return isConstant;
 		}
	}
}
