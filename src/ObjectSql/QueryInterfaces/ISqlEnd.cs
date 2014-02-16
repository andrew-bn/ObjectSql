using System.Data;

namespace ObjectSql.QueryInterfaces
{
	public interface ISqlEnd
	{
		IDbCommand Command { get; }
	}
}
