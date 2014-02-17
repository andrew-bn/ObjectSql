using System.Data;
using ObjectSql.QueryInterfaces;

namespace ObjectSql
{
	public interface IObjectSqlManager
	{
		ISql Query();
		IDbConnection CreateConnection();
	}
}
