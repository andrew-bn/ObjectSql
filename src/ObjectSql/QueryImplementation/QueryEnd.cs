using System.Data;
using System.Threading.Tasks;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryParts;
using ObjectSql.QueryInterfaces;
using System.Collections.Generic;

namespace ObjectSql.QueryImplementation
{
	public class QueryEnd : QueryBase, IStoredProcedure
	{
		public QueryEnd(QueryContext context)
			: base(context)
		{
		}

		public object ExecuteScalar()
		{
			return ExecutionManager.ExecuteScalar(Context);
		}

		public IStoredProcedureResultReader ExecuteReader()
		{
			return ExecutionManager.ExecuteReader<object>(Context);
		}

		public IDbCommand Command
		{
			get
			{
				QueryManager.PrepareQuery(Context);
				return Context.QueryEnvironment.Command;
			}
		}

		public int ExecuteNonQuery()
		{
			return ExecutionManager.ExecuteNonQuery(Context);
		}


		public IStoredProcedure<T> Returns<T>(object dbType)
		{
			Context.AddQueryPart(new StoredProcedureResultPart(typeof(T),dbType));
			return new QueryEnd<T>(Context);
		}
	}
}
