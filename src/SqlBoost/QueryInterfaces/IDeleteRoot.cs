using System;
using System.Linq.Expressions;

namespace SqlBoost.QueryInterfaces
{
	public interface IDeleteRoot
	{
		INonQueryEnd Delete<T>(Expression<Func<T, bool>> condition);
		INonQueryEnd Delete<T>();
	}
}
