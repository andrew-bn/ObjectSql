using System.Data;

namespace SqlBoost.Core
{
	public interface ISqlBoostConnection: IDbConnection
	{
		IDbConnection UnderlyingConnection { get; }
	}
}
