using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.SqlServer
{
	public class ObjectSqlSqlServerInitializer
	{
		private static bool _initialized = false;

		public static void Initialize()
		{
			if (_initialized) return;
			ObjectSqlRegistry.RegisterDatabaseManager(new SqlServerDatabaseManager());
			_initialized = true;
		}
	}
}
