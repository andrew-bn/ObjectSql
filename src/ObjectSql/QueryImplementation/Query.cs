using System.Data;
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
	public class Query : QueryBase, IQuery, IQueryEnd
	{
		public Query(QueryContext context)
			: base(context)
		{
		}
		public IQuery<TTable> From<TTable>()
		{
			Context.SqlPart.AddQueryPart(new FromPart(typeof(TTable)));
			return new Query<TTable>(Context);
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

		public IQueryEnd Delete<T>(Expression<Func<T, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new DeletePart(typeof(T)));
			Context.SqlPart.AddQueryPart(new WherePart(false, condition));
			return this;
		}
		public IQueryEnd Delete<T>()
		{
			Context.SqlPart.AddQueryPart(new DeletePart(typeof(T)));
			return this;
		}
		public IQueryEnd<TEntity> Exec<THolder, TEntity>(Expression<Func<THolder, IEnumerable<TEntity>>> spExecutor)
		{
			Context.SqlPart.AddQueryPart(new StoredProcedurePart(spExecutor, typeof(TEntity), true));
			return new QueryEnd<TEntity>(Context);
		}
		public IQueryEnd Exec<THolder>(Expression<Action<THolder>> spExecutor)
		{
			Context.SqlPart.AddQueryPart(new StoredProcedurePart(spExecutor, null, true));
			return this;
		}

		public object ExecuteScalar()
		{
			return ExecutionManager.ExecuteScalar(Context);
		}

		public IObjectDataReader ExecuteReader()
		{
			return ExecutionManager.ExecuteReader<object>(Context);
		}

		public IDbCommand Command
		{
			get
			{
				return Context.PrepareQuery();
			}
		}

		public IQuery NextObjectSql()
		{
			Context.SqlPart.AddQueryPart(new NextQueryPart());
			return this;
		}

		public int ExecuteNonQuery()
		{
			return ExecutionManager.ExecuteNonQuery(Context);
		}

		public IQueryEnd Returns<TResult>(object sqlDbType)
		{
			Context.SqlPart.AddQueryPart(new QueryResultPart(typeof(TResult),sqlDbType));
			return this;
		}
	}
}
