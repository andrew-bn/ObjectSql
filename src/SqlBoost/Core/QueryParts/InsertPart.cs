﻿using System.Linq.Expressions;

namespace SqlBoost.Core.QueryParts
{
	public class InsertPart : LambdaBasedQueryPart
	{
		public InsertPart(LambdaExpression expression)
			:base(expression)
		{
			
		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.Insert; }
		}
	}
}
