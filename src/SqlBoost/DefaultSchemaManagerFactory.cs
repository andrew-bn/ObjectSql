using System;
using System.Data;
using SqlBoost.Core.SchemaManager;

namespace SqlBoost
{
	public class DefaultSchemaManagerFactory: ISchemaManagerFactory
	{
		public bool MatchSchemaManager(IDbConnection connection, string connectionString)
		{
			return true;
		}

		public void SetupConnectionString(IDbConnection connection, string connectionString)
		{
			connection.ConnectionString = connectionString;
		}

		public IEntitySchemaManager CreateSchemaManager(string connectionString)
		{
			return new EntitySchemaManager<SqlDbType>();
		}


		public string TryGetProviderName(IDbConnection connection, string connectionString)
		{
			return string.Empty;
		}


		public IEntitySchemaManager CreateSchemaManager(System.Type dbType, string connectionString)
		{
			return (IEntitySchemaManager)
			       Activator.CreateInstance(typeof (EntitySchemaManager<>).MakeGenericType(dbType), connectionString);
		}
	}
}
