﻿using System.Linq.Expressions;

namespace ObjectSql.Core.QueryParts
{
	public class SelectPart: LambdaBasedQueryPart
	{
		public SelectPart(LambdaExpression expression)
			:base(expression)
		{
			
		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.Select; }
		}
	}
}