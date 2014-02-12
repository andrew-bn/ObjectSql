using System;
using SqlBoost.Core.Bo;

namespace SqlBoost.Core.QueryParts
{
	public class DeletePart : QueryPartBase
	{
		public Type Entity { get; private set; }
		public DeletePart(Type entity)
		{
			Entity = entity;
		}
		public override QueryPartType PartType
		{
			get { return QueryPartType.Delete; }
		}
		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.Hash *= PRIME;
			parameters.Hash ^= Entity.GetHashCode();
		}
		public override bool IsEqualTo(IQueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) && Entity == ((DeletePart)part).Entity;
		}
	}
}
