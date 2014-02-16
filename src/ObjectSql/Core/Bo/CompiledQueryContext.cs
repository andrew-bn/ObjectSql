using System.Data;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class CompiledQueryContext: QueryContext
	{
		internal CompiledQueryContext(IDbCommand command, IQueryBuilder queryBuilder, IEntitySchemaManager schemaManager, object newRootValue, QueryContext copyFromContext)
			: base(command, queryBuilder, schemaManager, copyFromContext.ConnectionString, copyFromContext.ResourcesTreatmentType)
		{

			QueryRootsStruct = copyFromContext.QueryRoots;
			Prepared = true;

			foreach(var part in copyFromContext.QueryParts)
				AddQueryPart(part);

			QueryRootsStruct.ClearRoots();
			QueryRootsStruct.AddRoot(newRootValue, -1);
		}
	}
}
