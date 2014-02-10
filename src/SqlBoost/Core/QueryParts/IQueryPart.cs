﻿using SqlBoost.Core.Bo;

namespace SqlBoost.Core.QueryParts
{
	internal interface IQueryPart
	{
		QueryPartType PartType { get; }
		void CalculateQueryExpressionParameters(ref QueryRoots parameters);
		bool IsEqualTo(IQueryPart part, ref QueryRoots rootsA, ref QueryRoots rootsB);
	}
}
