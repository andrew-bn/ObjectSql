using System;
using System.Linq.Expressions;

namespace ObjectSql.QueryInterfaces
{
	public interface ISelectRoot
	{
		ISource<TTable> From<TTable>();
		IQueryEnd<TValue> Select<TValue>(Expression<Func<TValue>> select);
	}
}
