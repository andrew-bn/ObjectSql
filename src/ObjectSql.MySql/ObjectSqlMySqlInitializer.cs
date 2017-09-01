using System;

namespace ObjectSql.MySql
{
	public class ObjectSqlMySqlInitializer
	{
		private static bool _initialized;

		public static void Initialize()
		{
			if (_initialized) return;
			ObjectSqlRegistry.RegisterDatabaseManager(new MySqlDatabaseManager());
			_initialized = true;
		}
	}
}
