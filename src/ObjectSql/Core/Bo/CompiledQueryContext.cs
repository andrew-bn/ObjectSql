using System.Data;
using System.Data.Common;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class CompiledQueryContext: QueryContext
	{
		internal CompiledQueryContext(string initialConnectionString,
								DbCommand command,
								ResourcesTreatmentType resourcesTreatmentType, QueryEnvironment environment, object newRootValue,int rootIndex, QueryContext copyFromContext)
			: base(initialConnectionString,command,resourcesTreatmentType, environment)
		{

			PreparationData = copyFromContext.PreparationData;
			SqlPart = copyFromContext.SqlPart;
			Prepared = true;

			SqlPart.QueryRoots.ReplaceRoot(rootIndex, newRootValue);
		}
	}
}
