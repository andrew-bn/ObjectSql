using System.Data;
using System.Data.Common;

namespace ObjectSql.Core
{
	public interface IObjectSqlConnection
	{
		DbConnection UnderlyingConnection { get; }
	}
}
