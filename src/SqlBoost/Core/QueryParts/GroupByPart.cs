using System.Linq.Expressions;

namespace SqlBoost.Core.QueryParts
{
	internal class GroupByPart:LambdaBasedQueryPart
	{
		public GroupByPart(LambdaExpression expression)
			:base(expression)
		{
			
		}
		public override QueryPartType PartType
		{
			get { return QueryPartType.GroupBy; }
		}
	}
}
