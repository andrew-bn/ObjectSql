using System.Data;

namespace SqlBoost.Core.Bo
{
	internal class CompiledQueryContext: QueryContext
	{
		internal CompiledQueryContext(IDbCommand command, object newRootValue, QueryContext copyFromContext)
			: base(command, copyFromContext.ConnectionString, copyFromContext.ResourcesTreatmentType)
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
