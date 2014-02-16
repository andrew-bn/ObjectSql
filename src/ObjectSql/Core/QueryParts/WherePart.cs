using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class WherePart: LambdaBasedQueryPart
	{
		public bool UseAliases { get; private set; }
		public WherePart(bool useAliases, LambdaExpression expression)
			:base(expression)
		{
			UseAliases = useAliases;
		}
		public override QueryPartType PartType
		{
			get { return QueryPartType.Where; }
		}
		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.Hash *= PRIME;
			parameters.Hash ^= UseAliases ? 1 : 0;
		}
		public override bool IsEqualTo(IQueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) && UseAliases == ((WherePart)part).UseAliases;
		}
	}
}
