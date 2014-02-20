using System.Data;
using System.Threading.Tasks;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;
using System.Collections.Generic;

namespace ObjectSql.QueryImplementation
{
	public class QueryEnd<T> : QueryEnd, IStoredProcedureEnd<T>, IQueryEnd<T>
	{
		public QueryEnd(QueryContext context)
			: base(context)
		{
		}

		public IEnumerable<T> ExecuteQuery()
		{
			return ExecutionManager.ExecuteQuery<T>(Context);
		}
#if NET45
		public Task<IAsyncEnumerable<T>> ExecuteQueryAsync()
		{
			return ExecutionManager.ExecuteQueryAsync<T>(Context);
		}
#endif

		public new IStoredProcedureResultReader<T> ExecuteReader()
		{
			return ExecutionManager.ExecuteReader<T>(Context);
		}
	}
}
