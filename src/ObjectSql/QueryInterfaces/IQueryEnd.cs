using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ObjectSql.QueryInterfaces
{
	public interface IQueryEnd: IQuery
	{
		IQueryEnd Returns<TResult>(object sqlDbType);
		INonQueryResultHolder ExecuteNonQuery();
		IScalarResultHolder ExecuteScalar();
		IDataReaderHolder ExecuteReader();
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
