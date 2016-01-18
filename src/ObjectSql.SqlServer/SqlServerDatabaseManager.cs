using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Core.Misc;
using System.Reflection;
namespace ObjectSql.SqlServer
{
	public class SqlServerDatabaseManager : IDatabaseManager
	{
		public bool MatchManager(DbConnection dbConnection, string providerName)
		{
			return providerName == "System.Data.SqlClient" || (dbConnection is SqlConnection);
		}

		public bool MatchManager(DbDataReader dataReader)
		{
			return dataReader is SqlDataReader;
		}

		public Type DbType
		{
			get { return typeof(SqlDbType); }
		}


		public IDelegatesBuilder CreateDelegatesBuilder()
		{
			return SqlServerDelegatesBuilder.Instance;
		}

		public SqlWriter CreateSqlWriter()
		{
			return SqlServerSqlWriter.Instance;
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
				return "uniqueidentifier";
			if (netType == typeof(int))
				return "int";
			if (netType == typeof(short))
				return "smallint";
			if (netType == typeof(XmlReader))
				return "xml";
			if (netType == typeof(byte))
				return "tinyint";
			if (netType == typeof(bool))
				return "bit";
			if (netType == typeof(string))
				return "nvarchar";
			if (netType == typeof(DateTime))
				return "datetime2";
			if (netType == typeof(DateTimeOffset))
				return "datetimeoffset";
			if (netType == typeof(decimal))
				return "money";
			if (netType == typeof(double))
				return "float";
			if (netType == typeof(float))
				return "real";
			if (netType == typeof(object))
				return "sql_variant";
			if (netType == typeof(TimeSpan))
				return "time";

			return "sql_variant";
		}
	}
}
