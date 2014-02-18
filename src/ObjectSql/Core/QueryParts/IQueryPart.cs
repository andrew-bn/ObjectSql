using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public interface IQueryPart
	{
		QueryPartType PartType { get; }
		void CalculateQueryExpressionParameters(ref QueryRoots parameters);
		bool IsEqualTo(IQueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB);
		void BuildPart(BuilderContext context);
	}
}
