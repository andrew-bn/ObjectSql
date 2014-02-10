namespace SqlBoost.Core.QueryBuilder.InfoExtractor
{
	internal interface IInsertionInfoExtractor
	{
		EntityInsertionInformation ExtractFrom(System.Linq.Expressions.Expression expression);
	}
}
