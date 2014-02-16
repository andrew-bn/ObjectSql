using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.QueryImplementation
{
	public class QueryBase: IQueryContextHolder
	{
		public QueryContext Context { get; private set; }

		protected QueryBase(QueryContext context)
		{
			Context = context;
		}
		protected void PrepareQuery()
		{
			QueryManager.PrepareQuery(Context);
		}
	}
}
