using System.Collections.Generic;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Core.SchemaManager.EntitySchema;
using ObjectSql.Exceptions;
using ObjectSql.QueryImplementation;
using ObjectSql.QueryInterfaces;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class QueryExpressionBuilder : ExpressionVisitor, ISqlQueryBuilder
	{
		private bool _valueWasNull;
		protected BuilderContext BuilderContext { get; private set; }
		protected IEntitySchemaManager SchemaManager { get; private set; }
		protected bool UseAliases { get; private set; }
		public IStorageFieldType DbTypeInContext { get; set; }
		protected ICommandPreparatorsHolder CommandPreparatorsHolder { get { return BuilderContext.Preparators; } }
		protected IDelegatesBuilder DelegatesBuilder { get; private set; }
		protected SqlWriter SqlWriter { get; private set; }
		protected CommandText Text { get; set; }
		protected ParameterExpression[] ExpressionParameters { get; set; }
		public QueryExpressionBuilder(IEntitySchemaManager schemaManager,
			IDelegatesBuilder expressionBuilder, SqlWriter sqlWriter)
		{
			DbTypeInContext = null;
			SchemaManager = schemaManager;
			DelegatesBuilder = expressionBuilder;
			SqlWriter = sqlWriter;
		}
		public virtual string BuildSql(BuilderContext context, ParameterExpression[] parameters, Expression expression, bool useAliases)
		{
			_valueWasNull = false;
			UseAliases = useAliases;
			BuilderContext = context;
			ExpressionParameters = parameters;
			return BuildSql(expression);
		}
		public string BuildSql(IStorageFieldType dbTypeInContext, Expression expression)
		{
			DbTypeInContext = dbTypeInContext;
			return BuildSql(expression);
		}
		public string BuildSql(Expression expression)
		{
			var buf = Text;
			Text = new CommandText();
			Visit(expression);
			var result = Text.ToString();
			Text = buf;
			return result;
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
			if (!node.ContainsSql())
				AddParameter(node);
			else
				SqlWriter.WriteExpression(this, BuilderContext, Text, node);

			return node;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			SqlWriter.WriteName(Text, node.Name);
			return node;
		}
		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			var buf = Text;

			var parameters = new List<string>();

			foreach (var item in node.Expressions)
			{
				Text = new CommandText();
				Visit(item);
				parameters.Add(Text.ToString());
			}

			Text = buf;
			Text.Append(string.Join(", ", parameters));

			return node;
		}
		protected override Expression VisitMember(MemberExpression node)
		{
			if (!node.ContainsSql())
			{
				AddParameter(node);
			}
			else if (node.Expression.NodeType == ExpressionType.Parameter)
			{
				var entityType = node.Member.DeclaringType;
				var aliasName = ((ParameterExpression)node.Expression).Name;
				var fieldName = node.Member.Name;

				WriteStorageFieldAccess(entityType, aliasName, fieldName);
			}
			else if (node.Expression is MemberExpression &&
				((MemberExpression)node.Expression).Expression.Type.IsGenericType &&
				((MemberExpression)node.Expression).Expression.Type.GetGenericTypeDefinition() == typeof(ParametersSubstitutor<>))
			{

				var entityType = ((MemberExpression)node.Expression).Expression.Type.GetGenericArguments()[0];
				var aliasName = ((IParameterSubstitutor)((ConstantExpression)((MemberExpression)node.Expression).Expression).Value).Name;
				var fieldName = node.Member.Name;

				WriteStorageFieldAccess(entityType, aliasName, fieldName);
			}
			else
			{
				SqlWriter.WriteExpression(this, BuilderContext, Text, node);
			}

			return node;
		}

		private void WriteStorageFieldAccess(Type entityType, string aliasName, string fieldName)
		{
			var entitySchema = SchemaManager.GetSchema(entityType);

			if (UseAliases)
			{
				SqlWriter.WriteName(Text, aliasName);
				SqlWriter.WriteNameResolve(Text);
			}

			var storageField = entitySchema.GetStorageField(fieldName);
			DbTypeInContext = storageField.DbType;
			SqlWriter.WriteName(Text, storageField.Name);
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
			if (!node.ContainsSql())
			{
				AddParameter(node);
			}
			else if (!typeof(IQuery).IsAssignableFrom(node.Type))//is nested query
			{
				SqlWriter.WriteExpression(this, BuilderContext, Text, node);
			}
			else
			{
				var nodes = ExpressionEnumerator.Enumerate(node).ToList();
				var rootNode = nodes.FirstOrDefault(n => (n is MemberExpression) &&
										  ((MemberExpression)n).Expression == null &&
										  ((MemberExpression)n).Member == typeof(Sql).GetProperty("Query"));

				var indexOfRoot = nodes.IndexOf(rootNode);
				var param = Expression.Parameter(typeof(Query));
				nodes[indexOfRoot] = param;
				Expression newNode = param;

				for (int i = indexOfRoot - 1; i >= 0; i--)
					nodes[i] = newNode = ((MethodCallExpression)nodes[i]).Update(newNode, ((MethodCallExpression)nodes[i]).Arguments);

				newNode = newNode.Visit<ParameterExpression>((v, e) => SubstituteParameter(ExpressionParameters, e));

				var exp = Expression.Lambda<Func<Query, IQueryEnd>>(newNode, param).Compile();
				var ctx = new QueryContext(BuilderContext.Context.InitialConnectionString,
										   BuilderContext.Context.Command, BuilderContext.Context.ResourcesTreatmentType,
										   BuilderContext.Context.QueryEnvironment);

				var q = new Query(ctx);
				exp(q);

				foreach (var root in BuilderContext.Context.SqlPart.QueryRoots.Roots)
					q.Context.SqlPart.QueryRoots.AddRoot(root);

				q.Context.SqlPart.BuilderContext.Preparators = BuilderContext.Preparators;

				q.Context.SqlPart.BuildPart();
				Text.Append(q.Context.SqlPart.BuilderContext.Text.ToString());
			}

			return node;
		}

		private Expression SubstituteParameter(IEnumerable<ParameterExpression> parameters, ParameterExpression node)
		{
			if (parameters.Contains(node))
			{
				var s = (IParameterSubstitutor)Activator.CreateInstance(typeof(ParametersSubstitutor<>).MakeGenericType(node.Type));
				s.Name = node.Name;
				return Expression.MakeMemberAccess(Expression.Constant(s, s.GetType()), s.GetType().GetProperty("Table"));
			}
			return node;
		}


		private CommandParameterPreProcessor GetParameterDescriptor(Expression accessor, IStorageFieldType dbTypeInContext)
		{
			var descriptor = CommandPreparatorsHolder.PreProcessors
											 .Select(p => p as CommandParameterPreProcessor)
											 .Where(p => p != null)
											 .SingleOrDefault(d => ExpressionComparer.AreEqual(d.ValueAccessorExp, accessor) && Equals(d.DbType, dbTypeInContext));

			if (descriptor == null)
			{
				var parameterName = "p" + CommandPreparatorsHolder.PreProcessors.Count(p => p is CommandParameterPreProcessor);

				if (accessor.Type.IsArray)
					parameterName += "_" + Guid.NewGuid().ToString().Replace("-", "");

				var initializer = accessor.Type.IsArray
									? CreateArrayParameterInitializer(parameterName, accessor, dbTypeInContext)
									: CreateParameterInitializer(parameterName, accessor, dbTypeInContext);

				descriptor = new CommandParameterPreProcessor(parameterName, dbTypeInContext, accessor, initializer);
				CommandPreparatorsHolder.AddPreProcessor(descriptor);
			}

			descriptor.RootIndex = accessor.IndexOfRoot(BuilderContext.Context.SqlPart.QueryRoots);

			return descriptor;
		}

		protected Action<IDbCommand, object> CreateParameterInitializer(string name, Expression accessor, IStorageFieldType dbTypeInContext)
		{
			return DelegatesBuilder.CreateDatabaseParameterFactoryAction(Expression.Constant(name, typeof(string)), accessor, dbTypeInContext, ParameterDirection.Input);
		}
		protected Action<IDbCommand, object> CreateArrayParameterInitializer(string name, Expression accessor, IStorageFieldType dbTypeInContext)
		{
			return DelegatesBuilder.CreateArrayParameters(name, accessor, dbTypeInContext, ParameterDirection.Input);
		}

		private void RenderDatabaseExtension(ConstantExpression node)
		{
			if (node.Type.IsEnum)
			{
				var member = node.Type.GetMember(node.Value.ToString())[0];
				var emitAttr = member.GetCustomAttribute(typeof(EmitAttribute)) as EmitAttribute;
				if (emitAttr != null)
					Text.Append(emitAttr.Value);
				else
					Text.Append(node.Value.ToString());
			}
		}
	}
}
