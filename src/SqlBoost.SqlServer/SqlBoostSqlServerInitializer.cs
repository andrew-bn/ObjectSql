using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost.SqlServer
{
	public class SqlBoostSqlServerInitializer
	{
		public static void Initialize()
		{
			SqlBoostManager.RegisterDatabaseManager(new SqlServerDatabaseManager());
		}
	}
}
