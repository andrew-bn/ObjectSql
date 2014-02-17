using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.InfoExtractor
{
	public interface IMaterializationInfoExtractor
	{
		EntityMaterializationInformation ExtractFrom(Expression expression);
	}
}
