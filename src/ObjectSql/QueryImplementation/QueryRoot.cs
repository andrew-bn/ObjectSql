using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryParts;
using ObjectSql.QueryInterfaces;
using ObjectSql.Core.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.QueryImplementation
{
	public class QueryRoot : QueryBase, ISql, INonQueryEnd
	{
		public QueryRoot(QueryContext context)
			: base(context)
		{
		}
		public ISource<TTable> From<TTable>()
		{
			Context.SqlPart.AddQueryPart(new FromPart(typeof(TTable)));
			return new Source<TTable>(Context);
		}
		public IQueryEnd<TValue> Select<TValue>(Expression<Func<TValue>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TValue>(Context);
		}
		public IInsert<TDst> Insert<TDst>()
			where TDst : class
		{
			Expression<Func<TDst, object>> selector = c => c;
			return Insert<TDst>(selector);
		}

		public IInsert<TDst> Insert<TDst>(Expression<Func<TDst, object>> selector)
			where TDst : class
		{
			Context.SqlPart.AddQueryPart(new InsertPart(selector));
			return new Insert<TDst>(Context);
		}


		public IUpdate<T> Update<T>(Expression<Func<T>> set)
		{
			Context.SqlPart.AddQueryPart(new UpdatePart(set));
			return new Update<T>(Context);
		}

		public INonQueryEnd Delete<T>(Expression<Func<T, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new DeletePart(typeof(T)));
			Context.SqlPart.AddQueryPart(new WherePart(false, condition));
			return this;
		}
		public INonQueryEnd Delete<T>()
		{
			Context.SqlPart.AddQueryPart(new DeletePart(typeof(T)));
			return this;
		}
		public IQueryEnd<TEntity> Exec<THolder, TEntity>(Expression<Func<THolder, IEnumerable<TEntity>>> spExecutor)
		{
			Context.SqlPart.AddQueryPart(new StoredProcedurePart(spExecutor, typeof(TEntity), true));
			return new QueryEnd<TEntity>(Context);
		}
		public IStoredProcedure With<THolder>(Expression<Action<THolder>> spExecutor)
		{
			Context.SqlPart.AddQueryPart(new StoredProcedurePart(spExecutor, null, true));
			return new QueryEnd(Context);
		}

		public int ExecuteNonQuery()
		{
			return ExecutionManager.ExecuteNonQuery(Context);
		}

		public System.Data.IDbCommand Command
		{
			get
			{
				return Context.PrepareQuery();
			}
		}
	}
}
