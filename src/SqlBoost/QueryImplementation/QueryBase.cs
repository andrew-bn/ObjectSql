using SqlBoost.Core;
using SqlBoost.Core.Bo;
using SqlBoost.QueryInterfaces;

namespace SqlBoost.QueryImplementation
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
