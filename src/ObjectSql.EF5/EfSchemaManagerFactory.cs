using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Core.Misc;

namespace ObjectSql.EF5
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

		public bool MatchSchemaManager(DbConnection connection, string connectionString)
		{
			if (connection is EntityConnection)
				return true;
			EntityConnectionStringBuilder entityStringBuilder;
			return TryGetEfConnectionString(connectionString, out entityStringBuilder);
		}

		public void SetupConnectionString(DbConnection connection, string connectionString)
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

		private readonly static ConcurrentDictionary<string, IEntitySchemaManager> _cache = new ConcurrentDictionary<string, IEntitySchemaManager>();
		public IEntitySchemaManager CreateSchemaManager(Type dbType, string connectionString)
		{
			return _cache.GetOrAdd(connectionString,
							   (cs) =>
							   (IEntitySchemaManager)
							   Activator.CreateInstance(typeof(EdmEntitySchemaManager<>).MakeGenericType(dbType), new object[] { cs }));
		}


		public string TryGetProviderName(DbConnection connection, string connectionString)
		{
			EntityConnectionStringBuilder entityStringBuilder;
			if (TryGetEfConnectionString(connectionString, out entityStringBuilder))
				return entityStringBuilder.Provider;
			return string.Empty;
		}
	}
}
