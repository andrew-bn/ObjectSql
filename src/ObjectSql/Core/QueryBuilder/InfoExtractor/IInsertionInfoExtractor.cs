namespace ObjectSql.Core.QueryBuilder.InfoExtractor
{
	public interface IInsertionInfoExtractor
	{
		EntityInsertionInformation ExtractFrom(System.Linq.Expressions.Expression expression);
	}
}
