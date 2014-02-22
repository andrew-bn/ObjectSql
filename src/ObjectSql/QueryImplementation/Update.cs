using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryParts;
using ObjectSql.QueryInterfaces;
using System;
using System.Linq.Expressions;

namespace ObjectSql.QueryImplementation
{

	public class Update<T> : QueryBase, IUpdate<T>
	{
		public Update(QueryContext context)
			: base(context)
		{
		}
		public INonQueryEnd Where(Expression<Func<T, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(false, condition));
			return new QueryRoot(Context);
		}

		public int ExecuteNonQuery()
		{
			return ExecutionManager.ExecuteNonQuery(Context);
		}

		public System.Data.IDbCommand Command
		{
			get { return Context.QueryEnvironment.Command; }
		}
	}
}
