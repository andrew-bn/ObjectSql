using System;
using System.Data;
using System.Data.SqlClient;
using SqlBoost.Core.QueryBuilder;
using SqlBoost.Core.SchemaManager;

namespace SqlBoost.SqlServer
{
	public class SqlServerDatabaseManager:IDatabaseManager
	{
		public bool MatchManager(IDbConnection dbConnection,  string providerName)
		{
			return providerName == "System.Data.EntityClient" || (dbConnection is SqlConnection);
		}
		public IQueryBuilder CreateQueryBuilder(IEntitySchemaManager schemaManager)
		{
			return new SqlServerQueryBuilder(schemaManager);
		}

		public Type DbType
		{
			get { return typeof (SqlDbType); }
		}
	}
}
