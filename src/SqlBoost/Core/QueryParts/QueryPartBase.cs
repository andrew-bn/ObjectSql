using SqlBoost.Core.Bo;

namespace SqlBoost.Core.QueryParts
{
	internal abstract class QueryPartBase:IQueryPart
	{
		public const int PRIME = 397;
		public abstract QueryPartType PartType {get;}

		public virtual void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			parameters.Hash *= PRIME;
			parameters.Hash ^= (int)PartType;
		}

		public virtual bool IsEqualTo(IQueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return PartType == part.PartType;
		}
	}
}
