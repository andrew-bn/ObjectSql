using System.Linq.Expressions;

namespace ObjectSql.Core.QueryParts
{
	public class GroupByPart:LambdaBasedQueryPart
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
