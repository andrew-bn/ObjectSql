using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryParts;

namespace ObjectSql.Core.QueryBuilder
{
	public interface IQueryBuilder
	{
		QueryPreparationData BuildQuery(IQueryPart[] parts);
	}
}
