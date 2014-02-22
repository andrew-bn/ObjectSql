using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryParts;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.QueryImplementation
{
	public class Insert<TDst>: QueryBase, IInsert<TDst>
		where TDst : class
	{
		public Insert(QueryContext context)
			: base(context)
		{
		}
		public IQueryEnd Values(params TDst[] values)
		{
			Context.SqlPart.AddQueryPart(new ValuesPart(typeof(TDst), values));
			return new Query(Context);
		}
	}
}
