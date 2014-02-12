using SqlBoost.Core.Bo;
using SqlBoost.Core.QueryParts;

namespace SqlBoost.Core.QueryBuilder
{
	public interface IQueryBuilder
	{
		QueryPreparationData BuildQuery(IQueryPart[] parts);
	}
}
