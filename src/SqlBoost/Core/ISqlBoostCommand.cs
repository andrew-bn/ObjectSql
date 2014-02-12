using System.Data;

namespace SqlBoost.Core
{
	public interface ISqlBoostCommand: IDbCommand
	{
		IDbCommand UnderlyingCommand { get; }
	}
}
