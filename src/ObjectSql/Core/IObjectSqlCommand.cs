using System.Data;

namespace ObjectSql.Core
{
	public interface IObjectSqlCommand: IDbCommand
	{
		IDbCommand UnderlyingCommand { get; }
	}
}
