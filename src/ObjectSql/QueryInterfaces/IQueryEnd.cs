using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ObjectSql.QueryInterfaces
{
	public interface IQueryDataReader: IQueryContextHolder, IDisposable
	{
		IEnumerable<T> MapResult<T>();
	}
	public interface IQueryEnd : ISqlEnd, IQueryContextHolder, INonQueryEnd
	{
		object ExecuteScalar();
		IQueryDataReader ExecuteReader();
	}
	public interface IQueryEnd<T> : IQueryEnd
	{
		IEnumerable<T> ExecuteQuery();
#if NET45
		Task<IAsyncEnumerable<T>> ExecuteQueryAsync();
#endif
	}
}
