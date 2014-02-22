using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryParts;
using ObjectSql.QueryInterfaces;
using System;
using System.Linq.Expressions;

namespace ObjectSql.QueryImplementation
{

	public class Update<T> : Query, IUpdate<T>
	{
		public Update(QueryContext context)
			: base(context)
		{
		}
		public IQueryEnd Where(Expression<Func<T, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(false, condition));
			return new Query(Context);
		}
	}
}
