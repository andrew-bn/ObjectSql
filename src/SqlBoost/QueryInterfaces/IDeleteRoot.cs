using System;
using System.Linq.Expressions;

namespace SqlBoost.QueryInterfaces
{
	public interface IDeleteRoot
	{
		INonQueryEnd Delete<T>(Expression<Func<ITargetDatabase, T, bool>> condition);
		INonQueryEnd Delete<T>();
	}
}
