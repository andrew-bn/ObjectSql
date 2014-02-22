using System;
using System.Data;
using System.Linq.Expressions;
using ObjectSql.QueryInterfaces;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
namespace ObjectSql.QueryImplementation
{

	public class Query<TSource1> : QueryBase, IQuery<TSource1>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1> Where(Expression<Func<TSource1, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1> GroupBy(Expression<Func<TSource1,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2> : QueryBase, IQuery<TSource1,TSource2>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2> Where(Expression<Func<TSource1,TSource2, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2> GroupBy(Expression<Func<TSource1,TSource2,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TSource2,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TSource2,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2,TSource3> : QueryBase, IQuery<TSource1,TSource2,TSource3>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2,TSource3> Where(Expression<Func<TSource1,TSource2,TSource3, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3> GroupBy(Expression<Func<TSource1,TSource2,TSource3,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TSource2,TSource3,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2,TSource3,TSource4> : QueryBase, IQuery<TSource1,TSource2,TSource3,TSource4>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2,TSource3,TSource4> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TSource2,TSource3,TSource4,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2,TSource3,TSource4,TSource5> : QueryBase, IQuery<TSource1,TSource2,TSource3,TSource4,TSource5>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TSource2,TSource3,TSource4,TSource5,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6> : QueryBase, IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7> : QueryBase, IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8> : QueryBase, IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9> : QueryBase, IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TNewSource, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new JoinPart(condition));
			return new Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TNewSource>(Context);
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

	public class Query<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10> : QueryBase, IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10>
	{
		public Query(QueryContext context)
			: base(context)
		{
		}

		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, bool>> condition)
		{
			Context.SqlPart.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,object>> groupBy)
		{
			Context.SqlPart.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TEntity>> select)
		{
			Context.SqlPart.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TEntity>(Context);
		}
	}

}
