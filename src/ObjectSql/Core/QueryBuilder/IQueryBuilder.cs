using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.QueryParts;

namespace ObjectSql.Core.QueryBuilder
{
	public interface IQueryBuilder
	{
		IDelegatesBuilder DelegatesBuilder { get; }
		QueryPreparationData BuildQuery(IQueryPart[] parts);
	}
}
