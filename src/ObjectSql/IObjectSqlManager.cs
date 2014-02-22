using System.Data;
using ObjectSql.QueryInterfaces;

namespace ObjectSql
{
	public interface IObjectSqlManager
	{
		IQuery Query();
		IDbConnection CreateConnection();
	}
}
