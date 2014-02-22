using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ObjectSql.QueryInterfaces
{
	public interface IQueryEnd
	{
		IQueryEnd Returns<TResult>(object sqlDbType);
		int ExecuteNonQuery();
		object ExecuteScalar();
		IObjectDataReader ExecuteReader();
		IDbCommand Command { get; }
	}

	public interface IQueryEnd<TEntity> : IQueryEnd
	{
		IEnumerable<TEntity> ExecuteQuery();
#if NET45
		Task<IAsyncEnumerable<TEntity>> ExecuteQueryAsync();
#endif
	}
}
