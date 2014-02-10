using System.Linq.Expressions;

namespace SqlBoost.Core.QueryBuilder.InfoExtractor
{
	internal interface IMaterializationInfoExtractor
	{
		EntityMaterializationInformation ExtractFrom(Expression expression);
	}
}
