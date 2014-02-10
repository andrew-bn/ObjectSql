using System.Linq.Expressions;

namespace SqlBoost.Core.QueryParts
{
	internal class JoinPart: LambdaBasedQueryPart
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
