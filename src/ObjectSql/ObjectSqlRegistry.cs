using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ObjectSql
{
	public static class ObjectSqlRegistry
	{
		internal static List<ISchemaManagerFactory> ProviderManager = new List<ISchemaManagerFactory>();
		internal static List<IDatabaseManager> DatabaseManager = new List<IDatabaseManager>();

		static ObjectSqlRegistry()
		{
			ProviderManager.Add(new DefaultSchemaManagerFactory());
		}

		public static void RegisterSqlProviderManager(ISchemaManagerFactory managerFactory)
		{
			ProviderManager.Insert(0, managerFactory);
		}
		public static void RegisterDatabaseManager(IDatabaseManager manager)
		{
			DatabaseManager.Add(manager);
		}

		public static ISchemaManagerFactory FindSchemaManagerFactory(DbConnection connection, string cs)
		{
			foreach (var p in ProviderManager)
			{
				if (p.MatchSchemaManager(connection, cs))
				{
					return p;
				}
			}
			throw new NotImplementedException();
		}

		public static IDatabaseManager FindDatabaseManager(DbConnection connection, string provider)
		{
			foreach (var m in DatabaseManager)
			{
				if (m.MatchManager(connection, provider))
					return m;
			}
			throw new NotImplementedException();
		}
		public static IDatabaseManager FindDatabaseManager(DbDataReader dataReader)
		{
			var result = DatabaseManager.FirstOrDefault(m => m.MatchManager(dataReader));
			if (result == null)
				throw new NotImplementedException();
			return result;
		}

	}
}
