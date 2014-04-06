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
		public virtual void Count(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(WriteMethodCall("COUNT", args.Skip(1).Select(context.BuildSql)));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void Avg(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(WriteMethodCall("AVG", args.Skip(1).Select(context.BuildSql)));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void Max(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(WriteMethodCall("MAX", args.Skip(1).Select(context.BuildSql)));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void Min(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(WriteMethodCall("MIN", args.Skip(1).Select(context.BuildSql)));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void In(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(" ({0} IN ({1})) ", context.BuildSql(args[1]), string.Join(", ", args.Skip(2).Select(a => context.BuildSql(a))));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void NotIn(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(" ({0} NOT IN ({1})) ", context.BuildSql(args[1]), string.Join(", ", args.Skip(2).Select(a => context.BuildSql(a))));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void Like(SqlWriterContext context, params Expression[] args)
		{
			var str = string.Format(" ({0} LIKE {1})", context.BuildSql(args[1]), context.BuildSql(args[2]));
			context.CommandText.Append(str);
		}
		[DeclaringType(typeof(Sql))]
		public virtual void NotLike(SqlWriterContext context, params Expression[] args)
		{
			var str = string.Format(" ({0} NOT LIKE {1})", context.BuildSql(args[1]), context.BuildSql(args[2]));
			context.CommandText.Append(str);
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
		public virtual CommandText WriteName(BuilderContext context, CommandText commandText,string alias, string name)
		{
			var useAlias = !string.IsNullOrWhiteSpace(alias)
			               && !(context.CurrentPart is InsertPart)
			               && context.Parts.MoveBackAndFind(context.CurrentPart, p => p is DeletePart) == null
						   && context.Parts.MoveBackAndFind(context.CurrentPart, p => p is UpdatePart) == null;

			if (!useAlias)
				return commandText.Append("[{0}]", name);
			return commandText.Append("[{0}].[{1}]",alias, name);
		}
		#endregion
		protected static string ParameterSql(ISqlQueryBuilder builder, Expression methodCall, Expression expression)
		{
			var mc = (MethodCallExpression) methodCall;

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
				var mc = (MethodCallExpression)expression;
				var m = FindMethod(mc.Method.Name, mc.Method.DeclaringType);
				if (m == null)
					throw new ObjectSqlException("method '" + mc.Method.Name + "' can't be rendered to SQL");
				
				m.Invoke(this, new object[] { new SqlWriterContext(expression, expressionVisitor, context, commandText), new Expression[] { mc.Object }.Concat(mc.Arguments).ToArray() });
			}
			else if (expression.NodeType == ExpressionType.Constant)
			{
				var c = expression as ConstantExpression;
				var m = FindMethod("Constant", c.Type);
				if (m == null)
					throw new ObjectSqlException("value '" + (c.Value ?? "null") + "' can't be rendered to SQL");

				m.Invoke(this, new [] { new SqlWriterContext(expression, expressionVisitor, context, commandText), c.Value });
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

		private MethodInfo FindMethod(string methodName, Type type)
		{
			var sqlWriter = GetType();
			foreach (var m in sqlWriter.GetMethods())
			{
				
				var attr = m.GetCustomAttribute(typeof(DeclaringTypeAttribute)) as DeclaringTypeAttribute;
				if (attr != null && m.Name == methodName && attr.Type == type)
					return m;
			}
			return null;
		}

	}
}
