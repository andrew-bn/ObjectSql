using System.Data;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class CompiledQueryContext: QueryContext
	{
		internal CompiledQueryContext(QueryEnvironment environment, object newRootValue, QueryContext copyFromContext)
			: base(environment)
		{

			PreparationData = copyFromContext.PreparationData;
			SqlPart = copyFromContext.SqlPart;
			Prepared = true;

			SqlPart.QueryRoots.ClearRoots();
			SqlPart.QueryRoots.AddRoot(newRootValue, -1);
		}
	}
}
