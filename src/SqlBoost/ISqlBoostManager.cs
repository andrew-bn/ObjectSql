using System.Data;
using SqlBoost.QueryInterfaces;

namespace SqlBoost
{
	public interface ISqlBoostManager
	{
		ISql Query();
		IDbConnection CreateConnection();
	}
}
