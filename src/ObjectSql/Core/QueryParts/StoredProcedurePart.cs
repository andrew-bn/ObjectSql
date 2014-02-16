using System;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryParts
{
	public class StoredProcedurePart: LambdaBasedQueryPart
	{
		public bool ReturnsCollection { get; private set; }
		public Type EntityType{get;private set;}
		public StoredProcedurePart(LambdaExpression expression,Type entityType, bool returnsCollection)
			:base(expression)
		{
			ReturnsCollection = returnsCollection;
			EntityType = entityType;
		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.StoredProcedure; }
		}
	}
}
