using System.Threading.Tasks;
using SqlBoost.Core;
using SqlBoost.Core.Bo;
using SqlBoost.QueryInterfaces;
using System.Collections.Generic;

namespace SqlBoost.QueryImplementation
{
	public class QueryEnd<T> : QueryBase, IQueryEnd<T>
	{
		public QueryEnd(QueryContext context)
			: base(context)
		{
		}

		public object ExecuteScalar()
		{
			return ExecutionManager.ExecuteScalar(Context);
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
		public System.Data.IDbCommand Command
		{
			get
			{
				PrepareQuery();
				return Context.DbCommand;
			}
		}
	}
}
