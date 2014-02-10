using System;
using System.Linq.Expressions;

namespace SqlBoost.QueryInterfaces
{
	public interface IUpdate<T> : INonQueryEnd
	{
		INonQueryEnd Where(Expression<Func<ITargetDatabase, T, bool>> condition);
	}
}
