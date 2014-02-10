using System.Data;

namespace SqlBoost.Core
{
	internal interface ISqlBoostCommand: IDbCommand
	{
		IDbCommand UnderlyingCommand { get; }
	}
}
