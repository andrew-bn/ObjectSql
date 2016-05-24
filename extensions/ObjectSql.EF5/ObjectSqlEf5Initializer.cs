using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.EF5
{
	public class ObjectSqlEf5Initializer
	{
		private static bool _initialized = false;

		public static void Initialize()
		{
			if (_initialized) return;
			ObjectSqlRegistry.RegisterSqlProviderManager(new EfSchemaManagerFactory());
			_initialized = true;
		}
	}
}
