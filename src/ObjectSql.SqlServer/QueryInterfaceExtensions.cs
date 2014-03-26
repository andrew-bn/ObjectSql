using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.SqlServer
{
	public static class QueryInterfaceExtensions
	{
		public static IInsert<T> Output<T, TOut>(this IInsert<T> insert, Expression<Func<T, T, TOut>>  outputSelector)
			where T:class 
		{
			insert.Context.SqlPart.AddQueryPart(new OutputPart(outputSelector));
			return insert;
		}
	}
}
