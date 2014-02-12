using System.Linq.Expressions;

namespace SqlBoost.Core.QueryBuilder.InfoExtractor
{
	public interface IMaterializationInfoExtractor
	{
		EntityMaterializationInformation ExtractFrom(Expression expression);
	}
}
