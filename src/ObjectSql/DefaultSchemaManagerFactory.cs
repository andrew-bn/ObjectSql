﻿using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql
{
	public class DefaultSchemaManagerFactory: ISchemaManagerFactory
	{

		public bool MatchSchemaManager(DbConnection connection, string connectionString)
		{
			return true;
		}

		public void SetupConnectionString(DbConnection connection, string connectionString)
		{
			connection.ConnectionString = connectionString;
		}

		public string TryGetProviderName(DbConnection connection, string connectionString)
		{
			return string.Empty;
		}

		private readonly static ConcurrentDictionary<string, IEntitySchemaManager> _cache = new ConcurrentDictionary<string, IEntitySchemaManager>();
		public IEntitySchemaManager CreateSchemaManager(Type dbType, string connectionString)
		{
			return _cache.GetOrAdd(connectionString,
			                       (cs) =>
			                       (IEntitySchemaManager)
								   Activator.CreateInstance(typeof(EntitySchemaManager<>).MakeGenericType(dbType)));
		}
	}
}
