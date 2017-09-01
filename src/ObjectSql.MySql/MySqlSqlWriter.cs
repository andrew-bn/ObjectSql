using System;
using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.SchemaManager.EntitySchema;

namespace ObjectSql.MySql
{
	public class MySqlSqlWriter : SqlWriter
	{
		public static MySqlSqlWriter Instance = new MySqlSqlWriter();

		private MySqlSqlWriter()
		{
		}

		public override string LeftBoundary { get; } = "`";

		public override string RightBoundary { get; } = "`";

		public override CommandText WriteUpdate(CommandText commandText, EntitySchema entity, string updateSql)
		{
			return commandText.Append("UPDATE {0} SET {1}", PrepareStorageName(entity), updateSql);
		}

		public override CommandText WriteDelete(CommandText commandText, EntitySchema entity)
		{
			return commandText.Append("DELETE FROM {0} ", PrepareStorageName(entity));
		}

		public override CommandText WriteInsert(CommandText commandText, EntitySchema entity, string fieldsSql)
		{
			return commandText.Append("INSERT INTO {0} ({1})", PrepareStorageName(entity), fieldsSql);
		}

		public override CommandText WriteFrom(CommandText commandText, EntitySchema entity)
		{
			return commandText.Append("FROM {0}", PrepareStorageName(entity));
		}

		public override CommandText WriteAlias(CommandText commandText, string aliasName)
		{
			return commandText.Append(" AS `{0}`", aliasName);
		}

		public override CommandText WriteJoin(CommandText commandText, EntitySchema entity, string alias, string conditionSql,
			JoinType joinType)
		{
			var join = "";
			if (joinType != JoinType.Inner)
				join = " " + joinType.ToString().ToUpper();

			return commandText.Append("{3} JOIN {0} AS `{1}` ON {2}", PrepareStorageName(entity), alias, conditionSql, join);
		}

		public override CommandText WriteWhere(CommandText commandText, string sql)
		{
			return commandText.Append(" WHERE {0}", sql);
		}

		public override CommandText WriteHaving(CommandText commandText, string sql)
		{
			return commandText.Append(" HAVING {0}", sql);
		}

		public override CommandText WriteGroupBy(CommandText commandText, string sql)
		{
			return commandText.Append(" GROUP BY {0}", sql);
		}

		public override CommandText WriteSelect(CommandText commandText, string sql)
		{
			return commandText.Append("SELECT {0} ", sql);
		}

		public CommandText WriteOutput(CommandText commandText, string sql)
		{
			return commandText.Append("OUTPUT {0} ", sql);
		}

		public override CommandText WriteProcedureCall(CommandText commandText, FuncSchema func)
		{
			return commandText.Append(PrepareStorageName(func.StorageName));
		}

		public override CommandText WriteParameter(CommandText commandText, string parameterName)
		{
			return commandText.Append("@{0}", parameterName);
		}

		public override CommandText WriteNull(CommandText commandText)
		{
			return commandText.Append("NULL");
		}

		public override CommandText WriteNameResolve(CommandText commandText)
		{
			return commandText.Append(".");
		}

		public override CommandText WriteBlock(CommandText commandText, string expression)
		{
			return commandText.Append("({0})", expression);
		}

		public override CommandText WriteNot(CommandText commandText, string condition)
		{
			return commandText.Append(" NOT {0}", condition);
		}

		public override CommandText WriteGreater(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} > {1}", left, right);
		}

		public override CommandText WriteGreaterOrEqual(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} >= {1}", left, right);
		}

		public override CommandText WriteLess(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} < {1}", left, right);
		}

		public override CommandText WriteLessOrEqual(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} <= {1}", left, right);
		}

		public override CommandText WriteAnd(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} AND {1}", left, right);
		}

		public override CommandText WriteOr(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} OR {1}", left, right);
		}

		public override CommandText WriteAdd(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} + {1}", left, right);
		}

		public override CommandText WriteSubtract(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} - {1}", left, right);
		}

		public override CommandText WriteDivide(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} / {1}", left, right);
		}

		public override CommandText WriteMultiply(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} * {1}", left, right);
		}

		public override CommandText WriteEqual(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} = {1}", left, right);
		}

		public override CommandText WriteEqualNull(CommandText commandText, string left)
		{
			return commandText.Append("{0} IS NULL", left);
		}

		public override CommandText WriteComma(CommandText commandText)
		{
			return commandText.Append(", ");
		}

		public override CommandText WriteNotEqual(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} <> {1}", left, right);
		}

		public override CommandText WriteNotEqualNull(CommandText commandText, string left)
		{
			return commandText.Append("{0} IS NOT NULL", left);
		}

		public override CommandText WriteSet(CommandText commandText)
		{
			return commandText.Append(" = ");
		}

		public override CommandText WriteSqlEnd(CommandText commandText)
		{
			return commandText.Append("; " + Environment.NewLine);
		}

		private static string PrepareStorageName(EntitySchema entitySchema)
		{
			return PrepareStorageName(entitySchema.StorageName);
		}

		private static string PrepareStorageName(StorageName storageName)
		{
			if (storageName.NameOnly)
			{
				return string.Format("{0}", storageName.Name);
			}

			return string.IsNullOrEmpty(storageName.Schema)
				? string.Format("`{0}`", storageName.Name)
				: string.Format("`{0}`.`{1}`", storageName.Schema, storageName.Name);
		}

		[DeclaringType(typeof(String))]
		public void ToUpper(SqlWriterContext context, MethodCallExpression methodCall)
		{
			var parameter = ParameterSql(context, methodCall.Object);
			context.CommandText.Append(string.Format("UPPER({0})", parameter));
		}

		[DeclaringType(typeof(String))]
		public void ToLower(SqlWriterContext context, MethodCallExpression methodCall)
		{
			var parameter = ParameterSql(context, methodCall.Object);
			context.CommandText.Append(string.Format("LOWER({0})", parameter));
		}

		[DeclaringType(typeof(MySql))]
		public void IsNull(SqlWriterContext context, MethodCallExpression methodCall)
		{
			var placeHolder = Guid.NewGuid().ToString().Replace("-", "");
			var parameter = ParameterSql(context, methodCall.Arguments.First());
			context.CommandText.Append($" ({placeHolder}{{0}} IS NULL{placeHolder}) ", parameter);

			context.Context.Preparators.PreProcessors.Add(new CommandPrePostProcessor((cmd, roots) =>
			{
				var startReplacement = cmd.CommandText.IndexOf($"{placeHolder}", StringComparison.Ordinal);
				var endReplacement = cmd.CommandText.LastIndexOf($"{placeHolder}", StringComparison.Ordinal);

				var substringToAnalize = cmd.CommandText.Substring(startReplacement, endReplacement - startReplacement);
				if (cmd.CommandText.Contains($" ({placeHolder} IS NULL") || substringToAnalize.Contains($"_"))
				{
					cmd.CommandText =
						cmd.CommandText.Substring(0, startReplacement) + "1=0" +
						cmd.CommandText.Substring(endReplacement + placeHolder.Length,
							cmd.CommandText.Length - endReplacement - placeHolder.Length);
				}
				else
				{
					cmd.CommandText = cmd.CommandText.Replace($"{placeHolder}", "");
				}
			}));
		}
	}
}