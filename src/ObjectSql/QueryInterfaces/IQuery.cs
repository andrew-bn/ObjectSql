using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ObjectSql.QueryInterfaces
{
	public interface IQuery
	{
		IQuery<TSource> From<TSource>();
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TEntity>> select);

		IInsert<TSource> Insert<TSource>(Expression<Func<TSource, object>> selector) where TSource : class;
		IInsert<TSource> Insert<TSource>() where TSource : class;

		IUpdate<TSource> Update<TSource>(Expression<Func<TSource>> set);

		IQueryEnd Delete<TSource>(Expression<Func<TSource, bool>> condition);
		IQueryEnd Delete<TSource>();

		IQueryEnd<TEntity> Exec<TSpHolder, TEntity>(Expression<Func<TSpHolder, IEnumerable<TEntity>>> spExecutor);
		IQueryEnd Exec<TSpHolder>(Expression<Action<TSpHolder>> spExecutor);
	}
}
