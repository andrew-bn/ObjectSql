using SqlBoost.Core;
using SqlBoost.Core.Bo;
using SqlBoost.Core.QueryParts;
using SqlBoost.QueryInterfaces;
using System;
using System.Linq.Expressions;

namespace SqlBoost.QueryImplementation
{

	internal class Update<T> : QueryBase, IUpdate<T>
	{
		public Update(QueryContext context)
			: base(context)
		{
		}
		public INonQueryEnd Where(Expression<Func<ITargetDatabase, T, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(false, condition));
			PrepareQuery();
			return new Sql(Context);
		}

		public int ExecuteNonQuery()
		{
			return ExecutionManager.ExecuteNonQuery(Context);
		}

		public System.Data.IDbCommand Command
		{
			get { return Context.DbCommand; }
		}
	}
}
