using System;
using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core.QueryBuilder;

namespace SqlBoost.SqlServer
{
	public class SqlServerSqlWriter : ISqlWriter
	{
		public CommandText WriteUpdate(CommandText commandText, EntitySchema entity, string updateSql)
		{
			return commandText.Append("UPDATE {0} SET {1}", PrepareStorageName(entity), updateSql);
		}
		public CommandText WriteDelete(CommandText commandText, EntitySchema entity)
		{
			return commandText.Append("DELETE FROM {0} ", PrepareStorageName(entity));
		}
		public CommandText WriteInsert(CommandText commandText, EntitySchema entity, string fieldsSql)
		{
			return commandText.Append("INSERT INTO {0} ({1})", PrepareStorageName(entity), fieldsSql);
		}

		public CommandText WriteFrom(CommandText commandText, EntitySchema entity)
		{
			return commandText.Append("FROM {0}", PrepareStorageName(entity));
		}

		public CommandText WriteAlias(CommandText commandText, string aliasName)
		{
			return commandText.Append(" AS [{0}]", aliasName);
		}

		public CommandText WriteJoin(CommandText commandText, EntitySchema entity, string alias, string conditionSql)
		{
			return commandText.Append("JOIN {0} AS [{1}] ON {2}", PrepareStorageName(entity), alias, conditionSql);
		}

		public CommandText WriteWhere(CommandText commandText, string sql)
		{
			return commandText.Append(" WHERE {0}", sql);
		}

		public CommandText WriteHaving(CommandText commandText, string sql)
		{
			return commandText.Append(" HAVING {0}", sql);
		}

		public CommandText WriteGroupBy(CommandText commandText, string sql)
		{
			return commandText.Append(" GROUP BY {0}", sql);
		}

		public CommandText WriteSelect(CommandText commandText, string sql)
		{
			return commandText.Append("SELECT {0} ", sql);
		}

		public CommandText WriteProcedureCall(CommandText commandText, FuncSchema func)
		{
			return commandText.Append(PrepareStorageName(func.StorageName));
		}

		public CommandText WriteParameter(CommandText commandText, string parameterName)
		{
			return commandText.Append("@{0}", parameterName);
		}

		public CommandText WriteNull(CommandText commandText)
		{
			return commandText.Append("NULL");
		}

		public CommandText WriteName(CommandText commandText, string name)
		{
			return commandText.Append("[{0}]", name);
		}

		public CommandText WriteNameResolve(CommandText commandText)
		{
			return commandText.Append(".");
		}
		public CommandText WriteBlock(CommandText commandText,string expression)
		{
			return commandText.Append("({0})", expression);
		}

		public CommandText WriteNot(CommandText commandText, string condition)
		{
			return commandText.Append(" NOT {0}", condition);
		}

		public CommandText WriteGreater(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} > {1}", left, right);
		}

		public CommandText WriteGreaterOrEqual(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} >= {1}", left, right);
		}

		public CommandText WriteLess(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} < {1}", left, right);
		}

		public CommandText WriteLessOrEqual(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} <= {1}", left, right);
		}

		public CommandText WriteAnd(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} AND {1}", left, right);
		}

		public CommandText WriteOr(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} OR {1}", left, right);
		}

		public CommandText WriteAdd(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} + {1}", left, right);
		}

		public CommandText WriteSubtract(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} - {1}", left, right);
		}

		public CommandText WriteDivide(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} / {1}", left, right);
		}

		public CommandText WriteMultiply(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} * {1}", left, right);
		}
		public CommandText WriteEqual(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} = {1}", left, right);
		}
		public CommandText WriteEqualNull(CommandText commandText, string left)
		{
			return commandText.Append("{0} IS NULL",left);
		}

		public CommandText WriteComma(CommandText commandText)
		{
			return commandText.Append(", ");
		}
		public CommandText WriteNotEqual(CommandText commandText, string left, string right)
		{
			return commandText.Append("{0} <> {1}", left, right);
		}

		public CommandText WriteNotEqualNull(CommandText commandText, string left)
		{
			return commandText.Append("{0} IS NOT NULL", left);
		}
		public CommandText WriteSet(CommandText commandText)
		{
			return commandText.Append(" = ");
		}
		private static string PrepareStorageName(EntitySchema entitySchema)
		{
			return PrepareStorageName(entitySchema.StorageName);
		}

		private static string PrepareStorageName(StorageName storageName)
		{
			return String.IsNullOrEmpty(storageName.Schema)
					? String.Format("[{0}]", storageName.Name)
					: String.Format("[{0}].[{1}]", storageName.Schema, storageName.Name);
		}
	}
}
