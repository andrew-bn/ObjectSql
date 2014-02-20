using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ObjectSql.QueryInterfaces
{
	public interface IQueryEnd : IQueryContextHolder, INonQueryEnd
	{
		object ExecuteScalar();
	}

	public interface IQueryEnd<T> : IQueryEnd
	{
		IEnumerable<T> ExecuteQuery();
#if NET45
		Task<IAsyncEnumerable<T>> ExecuteQueryAsync();
#endif
	}
}
