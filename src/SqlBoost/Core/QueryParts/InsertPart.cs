using System.Linq.Expressions;

namespace SqlBoost.Core.QueryParts
{
	internal class InsertPart : LambdaBasedQueryPart
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
