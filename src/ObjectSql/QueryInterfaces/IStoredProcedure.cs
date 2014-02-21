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
	public interface IStoredProcedureEnd<T> : IQueryEnd
	{
		IStoredProcedureResultReader<T> ExecuteReader();
	}

	public interface IStoredProcedureEnd : IQueryEnd
	{
		IStoredProcedureResultReader ExecuteReader();
		IStoredProcedureEnd<T> Returns<T>(object dbType);
	}

	public interface IStoredProcedure
	{
		IQueryEnd<TEntity> Exec<TSpHolder, TEntity>(Expression<Func<TSpHolder, IEnumerable<TEntity>>> spExecutor);
		IStoredProcedureEnd With<TSpHolder>(Expression<Action<TSpHolder>> spExecutor);
	}
}
