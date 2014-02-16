using System;
using System.Linq.Expressions;

namespace ObjectSql.QueryInterfaces
{
	public interface IInsertRoot
	{
		IInsert<TDst> Insert<TDst>(Expression<Func<TDst, object>> selector) where TDst: class;
		IInsert<TDst> Insert<TDst>() where TDst : class;
	}
}
