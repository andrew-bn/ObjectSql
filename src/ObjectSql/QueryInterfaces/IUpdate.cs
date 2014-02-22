using System;
using System.Linq.Expressions;

namespace ObjectSql.QueryInterfaces
{
	public interface IUpdate<TSource> : IQueryEnd
	{
		IQueryEnd Where(Expression<Func<TSource, bool>> condition);
	}
}
