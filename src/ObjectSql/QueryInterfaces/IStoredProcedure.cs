using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectSql.QueryInterfaces
{
	public interface IStoredProcedure
	{
		IQueryEnd<TEntity> StoredProcedure<THolder,TEntity>(Expression<Func<THolder,IEnumerable<TEntity>>> spExecutor);
		IQueryEnd StoredProcedure<THolder>(Expression<Action<THolder>> spExecutor);
	}
}
