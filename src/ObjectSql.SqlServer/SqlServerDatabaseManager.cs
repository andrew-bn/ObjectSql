using System;
using System.Data;
using System.Data.SqlClient;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.SqlServer
{
	public class SqlServerDatabaseManager:IDatabaseManager
	{
		public bool MatchManager(IDbConnection dbConnection,  string providerName)
		{
			return providerName == "System.Data.SqlClient" || (dbConnection is SqlConnection);
		}

		public bool MatchManager(IDataReader dataReader)
		{
			return dataReader is SqlDataReader;
		}

		public Type DbType
		{
			get { return typeof (SqlDbType); }
		}


		public IDelegatesBuilder CreateDelegatesBuilder()
		{
			return SqlServerDelegatesBuilder.Instance;
		}

		public SqlWriter CreateSqlWriter()
		{
			return SqlServerSqlWriter.Instance;
		}
	}
}
