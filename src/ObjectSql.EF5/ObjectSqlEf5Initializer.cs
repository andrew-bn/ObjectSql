using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.EF5
{
	public class ObjectSqlEf5Initializer
	{
		public static void Initialize()
		{
			ObjectSqlRegistry.RegisterSqlProviderManager(new EfSchemaManagerFactory());
		}
	}
}
