using System;
using System.Data;
using System.Data.SqlClient;
using ObjectSql.Core.QueryBuilder;
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
		public IQueryBuilder CreateQueryBuilder(IEntitySchemaManager schemaManager)
		{
			return new SqlServerQueryBuilder(schemaManager);
		}

		public Type DbType
		{
			get { return typeof (SqlDbType); }
		}


		public Core.QueryBuilder.LambdaBuilder.DelegatesBuilder CreateDelegatesBuilder()
		{
			return SqlServerDelegatesBuilder.Instance;
		}


	}
}
