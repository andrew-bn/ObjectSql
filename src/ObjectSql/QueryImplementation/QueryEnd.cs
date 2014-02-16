using System.Data;
using System.Threading.Tasks;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;
using System.Collections.Generic;

namespace ObjectSql.QueryImplementation
{
	public class QueryEnd : QueryBase, IQueryEnd
	{
		public QueryEnd(QueryContext context)
			: base(context)
		{
		}

		public object ExecuteScalar()
		{
			return ExecutionManager.ExecuteScalar(Context);
		}

		public IQueryDataReader ExecuteReader()
		{
			return ExecutionManager.ExecuteReader(Context);
		}

		public IDbCommand Command
		{
			get
			{
				PrepareQuery();
				return Context.DbCommand;
			}
		}
	}
}
