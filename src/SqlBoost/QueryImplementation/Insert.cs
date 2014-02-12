using SqlBoost.Core.Bo;
using SqlBoost.Core.QueryParts;
using SqlBoost.QueryInterfaces;

namespace SqlBoost.QueryImplementation
{
	public class Insert<TDst>: QueryBase, IInsert<TDst>
		where TDst : class
	{
		public Insert(QueryContext context)
			: base(context)
		{
		}
		public INonQueryEnd Values(params TDst[] values)
		{
			Context.AddQueryPart(new ValuesPart(typeof(TDst), values));
			PrepareQuery();
			return new Sql(Context);
		}
	}
}
