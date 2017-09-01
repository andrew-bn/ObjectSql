using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Xml;
using MySql.Data.MySqlClient;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;

namespace ObjectSql.MySql
{
	public class MySqlDatabaseManager : IDatabaseManager
	{
		public bool MatchManager(DbConnection dbConnection, string providerName)
		{
			return providerName == "System.Data.SqlClient" || (dbConnection is MySqlConnection);
		}

		public bool MatchManager(DbDataReader dataReader)
		{
			return dataReader is MySqlDataReader;
		}

		public Type DbType => typeof(MySqlDbType);


		public IDelegatesBuilder CreateDelegatesBuilder()
		{
			return MySqlDelegatesBuilder.Instance;
		}

		public SqlWriter CreateSqlWriter()
		{
			return MySqlSqlWriter.Instance;
		}


		public string MapToDbType(Type netType)
		{
			if (netType.IsGenericType() && netType.GetGenericTypeDefinition() == typeof(Nullable<>))
				netType = netType.GetGenericArguments()[0];

			if (netType == typeof(long))
				return "bigint";
			if (netType == typeof(byte[]))
				return "binary";
			if (netType == typeof(Guid))
				return "varchar";
			if (netType == typeof(int))
				return "int";
			if (netType == typeof(short))
				return "smallint";
			if (netType == typeof(XmlReader))
				return "xml";
			if (netType == typeof(byte))
				return "tinyint";
			if (netType == typeof(bool))
				return "tinyint(1)";
			if (netType == typeof(string))
				return "varchar";
			if (netType == typeof(DateTime))
				return "timestamp";
			if (netType == typeof(DateTimeOffset))
				return "datetimeoffset";
			if (netType == typeof(decimal))
				return "decimal";
			if (netType == typeof(double))
				return "double";
			if (netType == typeof(float))
				return "float";
			if (netType == typeof(TimeSpan))
				return "time";

			return "varchar";
		}
	}
}
