using System.Data;

namespace ObjectSql.Core
{
	public interface IObjectSqlConnection: IDbConnection
	{
		IDbConnection UnderlyingConnection { get; }
	}
}
