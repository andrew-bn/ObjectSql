using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectSql.QueryInterfaces
{
	public interface IStoredProcedure
	{
		IQueryEnd<TEntity> Exec<THolder,TEntity>(Expression<Func<THolder,IEnumerable<TEntity>>> spExecutor);
		IQueryEnd Exec<THolder>(Expression<Action<THolder>> spExecutor);
		IStoredProcedureHolder<THolder> Exec<THolder>();
	}
}
