using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SqlBoost.Core.Misc;
using SqlBoost.Core.SchemaManager;

namespace SqlBoost.EF5
{
	public class EfSchemaManagerFactory: ISchemaManagerFactory
	{
		private static readonly Regex _isEfConnectionString = new Regex("metadata=[^;]+;provider=[^;]+;provider connection string=\"[^\"]+\"");
		private static bool TryGetEfConnectionString(string connectionString, out EntityConnectionStringBuilder stringBuilder)
		{
			stringBuilder = null;

			if (_isEfConnectionString.IsMatch(connectionString))
				stringBuilder = new EntityConnectionStringBuilder(connectionString);

			return stringBuilder != null;
		}

		public bool MatchSchemaManager(IDbConnection connection, string connectionString)
		{
			if (connection is EntityConnection)
				return true;
			EntityConnectionStringBuilder entityStringBuilder;
			return TryGetEfConnectionString(connectionString, out entityStringBuilder);
		}

		public void SetupConnectionString(IDbConnection connection, string connectionString)
		{
			if (connection is EntityConnection)
			{
				connection.ConnectionString = connectionString;
			}
			else
			{
				EntityConnectionStringBuilder entityStringBuilder;
				if (TryGetEfConnectionString(connectionString, out entityStringBuilder))
					connection.ConnectionString = entityStringBuilder.ProviderConnectionString;
			}
		}

		public IEntitySchemaManager CreateSchemaManager(Type dbType, string connectionString)
		{
			return
				(IEntitySchemaManager)
				Activator.CreateInstance(typeof (EdmEntitySchemaManager<>).MakeGenericType(dbType), new object[] {connectionString});
		}


		public string TryGetProviderName(IDbConnection connection, string connectionString)
		{
			EntityConnectionStringBuilder entityStringBuilder;
			if (TryGetEfConnectionString(connectionString, out entityStringBuilder))
				return entityStringBuilder.Provider;
			return string.Empty;
		}
	}
}
