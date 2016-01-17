using System.Data;
using System.Data.Common;
using ObjectSql.QueryInterfaces;

namespace ObjectSql
{
	public interface IObjectSqlManager
	{
		IQuery Query();
		DbConnection CreateConnection();
	}
}
