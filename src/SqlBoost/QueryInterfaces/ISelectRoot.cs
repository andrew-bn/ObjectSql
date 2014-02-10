using System;
using System.Linq.Expressions;

namespace SqlBoost.QueryInterfaces
{
	public interface ISelectRoot
	{
		ISource<TTable> From<TTable>();
		IQueryEnd<TValue> Select<TValue>(Expression<Func<ITargetDatabase, TValue>> select);
	}
}
