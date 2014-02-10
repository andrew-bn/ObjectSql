using SqlBoost.Core.Bo;
using SqlBoost.Core.QueryParts;

namespace SqlBoost.Core.QueryBuilder
{
	internal interface IQueryBuilder
	{
		QueryPreparationData BuildQuery(IQueryPart[] parts);
	}
}
