using System.Data;

namespace SqlBoost.Core
{
	internal interface ISqlBoostConnection: IDbConnection
	{
		IDbConnection UnderlyingConnection { get; }
	}
}
