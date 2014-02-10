using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SqlBoost.QueryInterfaces
{
	public interface IStoredProcedure
	{
		IQueryEnd<TEntity> StoredProcedure<THolder,TEntity>(Expression<Func<THolder,IEnumerable<TEntity>>> spExecutor);
	}
}
