using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace ObjectSql.QueryInterfaces
{
	public interface IQueryEnd: IQuery
	{
		IQueryEnd Returns<TResult>(object sqlDbType);
		INonQueryResultHolder ExecuteNonQuery();
		IScalarResultHolder ExecuteScalar();
		IDataReaderHolder ExecuteReader();
		DbCommand Command { get; }
	}

	public interface IQueryEnd<TEntity> : IQueryEnd
	{
		IEnumerable<TEntity> ExecuteQuery();
#if !NET40
		Task<IAsyncEnumerable<TEntity>> ExecuteQueryAsync();
#endif
	}
}
