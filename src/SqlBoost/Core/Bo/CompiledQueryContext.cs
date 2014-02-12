using System.Data;

namespace SqlBoost.Core.Bo
{
	public class CompiledQueryContext: QueryContext
	{
		internal CompiledQueryContext(IDbCommand command, IDatabaseManager dbManager, ISchemaManagerFactory schemaManagerFactory, object newRootValue, QueryContext copyFromContext)
			: base(command, dbManager, schemaManagerFactory, copyFromContext.ConnectionString, copyFromContext.ResourcesTreatmentType)
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
