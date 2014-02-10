using SqlBoost.Core;
using SqlBoost.Core.Bo;
using SqlBoost.QueryInterfaces;

namespace SqlBoost.QueryImplementation
{
	internal class QueryBase: IQueryContextHolder
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
