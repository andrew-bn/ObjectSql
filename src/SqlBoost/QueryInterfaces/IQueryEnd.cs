using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlBoost.QueryInterfaces
{
	public interface IQueryEnd<T>: ISqlEnd, IQueryContextHolder
	{
		object ExecuteScalar();
		IEnumerable<T> ExecuteQuery();
#if NET45
		Task<IAsyncEnumerable<T>> ExecuteQueryAsync();
#endif
	}
}
