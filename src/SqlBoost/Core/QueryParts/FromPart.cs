using System;
using SqlBoost.Core.Bo;

namespace SqlBoost.Core.QueryParts
{
	public class FromPart: QueryPartBase
	{
		public Type[] Entities { get; private set; }
		public FromPart(params Type[] entities)
		{
			Entities = entities;
		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.From; }
		}
		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			foreach (var type in Entities)
			{
				parameters.Hash *= PRIME;
				parameters.Hash ^= type.GetHashCode();
			}
		}
		public override bool IsEqualTo(IQueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB)
		{
			if (!base.IsEqualTo(part, ref rootsA, ref rootsB))
				return false;
			var b = (FromPart)part;
			if (Entities.Length != b.Entities.Length)
				return false;
			for (int i = 0; i < Entities.Length; i++)
			{
				if (Entities[i] != b.Entities[i])
					return false;
			}
			return true;
		}
	}
}
