using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectSql.QueryInterfaces
{

	public interface IStoredProcedureResultReader : IQueryContextHolder, IDisposable
	{
		IEnumerable<TData> MapResult<TData>();
	}
	public interface IStoredProcedureResultReader<T> :IStoredProcedureResultReader
	{
		T ReturnValue { get; }
	}
	public interface IStoredProcedure<T> : IQueryEnd
	{
		IStoredProcedureResultReader<T> ExecuteReader();
	}

	public interface IStoredProcedure : IQueryEnd
	{
		IStoredProcedureResultReader ExecuteReader();
		IStoredProcedure<T> Returns<T>(object dbType);
	}

	public interface IStoredProcedureRoot
	{
		IQueryEnd<TEntity> Exec<TSpHolder, TEntity>(Expression<Func<TSpHolder, IEnumerable<TEntity>>> spExecutor);
		IStoredProcedure With<TSpHolder>(Expression<Action<TSpHolder>> spExecutor);
	}
}
