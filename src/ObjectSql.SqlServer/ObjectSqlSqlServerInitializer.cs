using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.SqlServer
{
	public class ObjectSqlSqlServerInitializer
	{
		public static void Initialize()
		{
			ObjectSqlManager.RegisterDatabaseManager(new SqlServerDatabaseManager());
		}
	}
}
