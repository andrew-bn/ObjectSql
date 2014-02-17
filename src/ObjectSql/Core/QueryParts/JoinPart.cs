﻿using System.Linq.Expressions;

namespace ObjectSql.Core.QueryParts
{
	public class JoinPart: LambdaBasedQueryPart
	{
		public JoinPart(LambdaExpression expression)
			:base(expression)
		{
			
		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.Join; }
		}
	}
}