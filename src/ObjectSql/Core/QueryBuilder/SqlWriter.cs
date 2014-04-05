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
		public abstract CommandText WriteName(CommandText commandText, string name);
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


		[DeclaringType(typeof(Sql))]
		public virtual void Count(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(BuildSql("COUNT", args.Skip(1).Select(context.BuildSql)));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void Avg(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(BuildSql("AVG", args.Skip(1).Select(context.BuildSql)));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void Max(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(BuildSql("MAX", args.Skip(1).Select(context.BuildSql)));
		}
		[DeclaringType(typeof(Sql))]
		public virtual void Min(SqlWriterContext context, params Expression[] args)
		{
			context.CommandText.Append(BuildSql("MIN", args.Skip(1).Select(context.BuildSql)));
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
		protected static string BuildSql(string method, IEnumerable<string> parts)
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

				m.Invoke(this, new object[] { new SqlWriterContext(expression, expressionVisitor, context, commandText), c.Value });
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
