using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core.SchemaManager.EntitySchema;
using ObjectSql.Exceptions;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;

namespace ObjectSql.Core.QueryBuilder
{
	public abstract class SqlWriter
	{
		public abstract CommandText WriteUpdate(CommandText commandText, EntitySchema entity, string updateSql);
		public abstract CommandText WriteDelete(CommandText commandText, EntitySchema entity);
		public abstract CommandText WriteInsert(CommandText commandText, EntitySchema entity, string fieldsSql);
		public abstract CommandText WriteFrom(CommandText commandText, EntitySchema entity);
		public abstract CommandText WriteAlias(CommandText commandText, string aliasName);
		public abstract CommandText WriteJoin(CommandText commandText, EntitySchema entity, string alias, string conditionSql, JoinType joinType);
		public abstract CommandText WriteWhere(CommandText commandText, string sql);
		public abstract CommandText WriteHaving(CommandText commandText, string sql);
		public abstract CommandText WriteGroupBy(CommandText commandText, string sql);
		public abstract CommandText WriteSelect(CommandText commandText, string sql);
		public abstract CommandText WriteProcedureCall(CommandText commandText, FuncSchema func);
		public abstract CommandText WriteParameter(CommandText commandText, string parameterName);
		public abstract CommandText WriteNull(CommandText commandText);
		public abstract CommandText WriteComma(CommandText commandText);
		public abstract CommandText WriteNameResolve(CommandText commandText);
		public abstract CommandText WriteBlock(CommandText commandText, string expression);
		public abstract CommandText WriteNot(CommandText commandText, string condition);
		public abstract CommandText WriteGreater(CommandText commandText, string left, string right);
		public abstract CommandText WriteGreaterOrEqual(CommandText commandText, string left, string right);
		public abstract CommandText WriteLess(CommandText commandText, string left, string right);
		public abstract CommandText WriteLessOrEqual(CommandText commandText, string left, string right);
		public abstract CommandText WriteAnd(CommandText commandText, string left, string right);
		public abstract CommandText WriteOr(CommandText commandText, string left, string right);
		public abstract CommandText WriteAdd(CommandText commandText, string left, string right);
		public abstract CommandText WriteSubtract(CommandText commandText, string left, string right);
		public abstract CommandText WriteDivide(CommandText commandText, string left, string right);
		public abstract CommandText WriteMultiply(CommandText commandText, string left, string right);
		public abstract CommandText WriteEqual(CommandText commandText, string left, string right);
		public abstract CommandText WriteEqualNull(CommandText commandText, string left);
		public abstract CommandText WriteNotEqual(CommandText commandText, string left, string right);
		public abstract CommandText WriteNotEqualNull(CommandText commandText, string left);
		public abstract CommandText WriteSet(CommandText commandText);
		public abstract CommandText WriteSqlEnd(CommandText commandText);

		#region sql 
		[DeclaringType(typeof(Sql))]
		public virtual void Count(SqlWriterContext context, MethodCallExpression methodCall)
		{
			context.CommandText.Append(WriteMethodCall("COUNT",
			methodCall.Arguments.Select(a =>
			{
				context.UpdateTypeInContext("");
				return context.BuildSql(a);
			})));
			var dbType = context.Context.DatabaseManager.MapToDbType(typeof(int));
			context.UpdateTypeInContext(dbType);
		}
		#endregion sql
		#region Binary
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void Equal(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Empty;
			if (right.StripConvert().NodeType == ExpressionType.Constant && ((ConstantExpression)right.StripConvert()).Value == null)
				str = string.Format(" ({0} IS NULL)", context.BuildSql(left));

			else if (left.StripConvert().NodeType == ExpressionType.Constant && ((ConstantExpression)left.StripConvert()).Value == null)
				str = string.Format(" ({0} IS NULL)", context.BuildSql(right));

			else str = string.Format(" ({0} = {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void NotEqual(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Empty;
			if (right.StripConvert().NodeType == ExpressionType.Constant && ((ConstantExpression)right.StripConvert()).Value == null)
				str = string.Format(" ({0} IS NOT NULL)", context.BuildSql(left));

			else if (left.StripConvert().NodeType == ExpressionType.Constant && ((ConstantExpression)left.StripConvert()).Value == null)
				str = string.Format(" ({0} IS NOT NULL)", context.BuildSql(right));

			else str = string.Format(" ({0} <> {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void AndAlso(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} AND {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void Subtract(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} - {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void Divide(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} / {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void GreaterThan(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} > {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void GreaterThanOrEqual(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} >= {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void LessThan(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} < {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void LessThanOrEqual(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} <= {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void Multiply(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} * {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void OrElse(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} OR {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(BinaryExpression))]
		public virtual void Add(SqlWriterContext context, Expression left, Expression right)
		{
			var str = string.Format(" ({0} + {1})", context.BuildSql(left), context.BuildSql(right));
			context.CommandText.Append(str);
		}

		[DeclaringType(typeof(Sql))]
		public virtual void In(SqlWriterContext context, MethodCallExpression callExpression)
		{
			var placeHolder = Guid.NewGuid().ToString().Replace("-", "");
			RenderMethodCallToSql(context, callExpression, $" ({placeHolder}{{0}} IN ({{1}}){placeHolder}) ");

			context.Context.Preparators.PreProcessors.Add(new CommandPrePostProcessor((cmd, roots) =>
			{
				if (cmd.CommandText.Contains($" IN (){placeHolder})") || cmd.CommandText.Contains($" IN (NULL){placeHolder})"))
				{
					var startReplacement = cmd.CommandText.IndexOf($"{placeHolder}");
					var endReplacement = cmd.CommandText.LastIndexOf($"{placeHolder}");

					cmd.CommandText =
						cmd.CommandText.Substring(0, startReplacement) + "1=0" + 
						cmd.CommandText.Substring(endReplacement + placeHolder.Length, cmd.CommandText.Length - endReplacement - placeHolder.Length);
				}
				else
				{
					cmd.CommandText = cmd.CommandText.Replace($"{placeHolder}", "");
				}
			}));
		}

		#endregion Binary
		#region Unary
		[DeclaringType(typeof(UnaryExpression))]
		public virtual void Not(SqlWriterContext context, Expression operand)
		{
			var str = string.Format("(NOT {0})", context.BuildSql(operand));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(UnaryExpression))]
		public virtual void Convert(SqlWriterContext context, Expression operand)
		{
			context.CommandText.Append(context.BuildSql(operand));
		}
		#endregion Unary

		#region common
		public virtual CommandText WriteName(BuilderContext context, CommandText commandText, string alias, string name)
		{
			var useAlias = !string.IsNullOrWhiteSpace(alias)
						   && !(context.CurrentPart is InsertPart)
						   && context.Parts.MoveBackAndFind(context.CurrentPart, p => p is DeletePart) == null
						   && context.Parts.MoveBackAndFind(context.CurrentPart, p => p is UpdatePart) == null;

			if (!useAlias)
				return commandText.Append("[{0}]", name);
			return commandText.Append("[{0}].[{1}]", alias, name);
		}
		#endregion

		protected static string ParameterSql(SqlWriterContext builder, Expression expression)
		{
			var dbType = builder.Context.DatabaseManager.MapToDbType(expression.Type);
			builder.UpdateTypeInContext(dbType);
			return builder.BuildSql(expression);
		}

		protected static string WriteMethodCall(string method, IEnumerable<string> parts)
		{
			return string.Format(" {0}({1}) ", method, string.Join(", ", parts));
		}

		public CommandText WriteExpression(ISqlQueryBuilder expressionVisitor, Core.Bo.BuilderContext context, CommandText commandText, System.Linq.Expressions.Expression expression)
		{
			if (expression.NodeType == ExpressionType.Call)
			{
				var sqlContext = new SqlWriterContext(expression, expressionVisitor, context, commandText);
				var mc = (MethodCallExpression)expression;
				var sqlAttr = mc.Method.GetCustomAttr<SqlAttribute>();
				var m = FindMethod(mc.Method.Name, mc.Method.DeclaringType);
				if (m == null && sqlAttr == null)
				{
					throw new ObjectSqlException("method '" + mc.Method.Name + "' can't be rendered to SQL");
				}
				if (m != null)
				{
					m.Invoke(this, new object[] { sqlContext, mc });
				}
				else
				{
					RenderMethodCallToSql(sqlContext, mc, sqlAttr);
				}

				if (mc.Method.ReturnType != typeof(void))
				{
					var dbType = context.DatabaseManager.MapToDbType(mc.Method.ReturnType);
					sqlContext.UpdateTypeInContext(dbType);
				}
			}
			else if (expression.NodeType == ExpressionType.Constant)
			{
				var c = expression as ConstantExpression;
				var m = FindMethod("Constant", c.Type);
				if (m == null)
				{
					throw new ObjectSqlException("value '" + (c.Value ?? "null") + "' can't be rendered to SQL");
				}

				m.Invoke(this, new[] { new SqlWriterContext(expression, expressionVisitor, context, commandText), c.Value });
			}
			else if (expression is BinaryExpression)
			{
				var bi = expression as BinaryExpression;
				var m = FindMethod(expression.NodeType.ToString(), typeof(BinaryExpression));
				if (m == null)
					throw new ObjectSqlException("binary expression '" + expression.NodeType.ToString() + "' can't be rendered to SQL");

				m.Invoke(this, new object[] { new SqlWriterContext(expression, expressionVisitor, context, commandText), bi.Left, bi.Right });
			}
			else if (expression is UnaryExpression)
			{
				var un = expression as UnaryExpression;
				var m = FindMethod(expression.NodeType.ToString(), typeof(UnaryExpression));
				if (m == null)
					throw new ObjectSqlException("unary expression '" + expression.NodeType.ToString() + "' can't be rendered to SQL");

				m.Invoke(this, new object[] { new SqlWriterContext(expression, expressionVisitor, context, commandText), un.Operand });

			}

			return commandText;
		}

		protected void RenderMethodCallToSql(SqlWriterContext context, MethodCallExpression expression, SqlAttribute sqlAttr)
		{
			RenderMethodCallToSql(context, expression, sqlAttr.Pattern);
		}

		private void RenderMethodCallToSql(SqlWriterContext context, MethodCallExpression expression, string pattern)
		{
			var parameters = expression.Arguments.Select(a => ParameterSql(context, a)).ToArray();
			var sqlToAppend = string.Format(pattern, parameters);
			context.CommandText.Append(sqlToAppend);
		}

		private MethodInfo FindMethod(string methodName, Type type)
		{
			var sqlWriter = GetType();
			foreach (var m in sqlWriter.GetMethods())
			{
				var attr = m.GetCustomAttr(typeof(DeclaringTypeAttribute)) as DeclaringTypeAttribute;
				if (attr != null && m.Name == methodName && attr.Type == type)
					return m;
			}
			return null;
		}

	}
}
