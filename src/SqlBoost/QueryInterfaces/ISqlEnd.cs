using System.Data;

namespace SqlBoost.QueryInterfaces
{
	public interface ISqlEnd
	{
		IDbCommand Command { get; }
	}
}
