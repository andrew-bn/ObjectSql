using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost.EF5
{
	public class SqlBoostEf5Initializer
	{
		public static void Initialize()
		{
			SqlBoostManager.RegisterSqlProviderManager(new EfSchemaManagerFactory());
		}
	}
}
