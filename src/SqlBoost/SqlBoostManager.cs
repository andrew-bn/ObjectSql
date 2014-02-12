using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost
{
	public abstract class SqlBoostManager
	{
		internal static List<ISchemaManagerFactory> ProviderManager = new List<ISchemaManagerFactory>();
		internal static List<IDatabaseManager> DatabaseManager = new List<IDatabaseManager>();
		
		static SqlBoostManager()
		{
			ProviderManager.Add(new DefaultSchemaManagerFactory());
		}
		public static ISchemaManagerFactory FindSchemaManagerFactory(IDbConnection connection, string cs)
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
		public static IDatabaseManager FindDatabaseManager(IDbConnection connection, string provider)
		{
			foreach (var m in DatabaseManager)
			{
				if (m.MatchManager(connection, provider))
					return m;
			}
			throw new NotImplementedException();
		}
		public static void RegisterSqlProviderManager(ISchemaManagerFactory managerFactory)
		{
			ProviderManager.Insert(0, managerFactory);
		}
		public static void RegisterDatabaseManager(IDatabaseManager manager)
		{
			DatabaseManager.Add(manager);
		}
	}
}
