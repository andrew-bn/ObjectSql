using System;
using SqlBoost.Core.Bo;
using SqlBoost.QueryInterfaces;
using SqlBoost.Core.QueryParts;

namespace SqlBoost.QueryImplementation
{

	internal class Source<T1> : QueryBase, ISource<T1>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2> Join<T2>(System.Linq.Expressions.Expression<Func<T1,T2, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2> : QueryBase, ISource<T1,T2>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2,T3> Join<T3>(System.Linq.Expressions.Expression<Func<T1,T2,T3, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2,T3>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2,T3> : QueryBase, ISource<T1,T2,T3>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2,T3> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2,T3> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2,T3,T4> Join<T4>(System.Linq.Expressions.Expression<Func<T1,T2,T3,T4, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2,T3,T4>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2,T3,T4> : QueryBase, ISource<T1,T2,T3,T4>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2,T3,T4> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2,T3,T4> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5> Join<T5>(System.Linq.Expressions.Expression<Func<T1,T2,T3,T4,T5, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2,T3,T4,T5>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2,T3,T4,T5> : QueryBase, ISource<T1,T2,T3,T4,T5>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2,T3,T4,T5> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6> Join<T6>(System.Linq.Expressions.Expression<Func<T1,T2,T3,T4,T5,T6, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2,T3,T4,T5,T6>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2,T3,T4,T5,T6> : QueryBase, ISource<T1,T2,T3,T4,T5,T6>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2,T3,T4,T5,T6> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6,T7> Join<T7>(System.Linq.Expressions.Expression<Func<T1,T2,T3,T4,T5,T6,T7, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2,T3,T4,T5,T6,T7>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2,T3,T4,T5,T6,T7> : QueryBase, ISource<T1,T2,T3,T4,T5,T6,T7>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2,T3,T4,T5,T6,T7> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6,T7> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6,T7,T8> Join<T8>(System.Linq.Expressions.Expression<Func<T1,T2,T3,T4,T5,T6,T7,T8, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2,T3,T4,T5,T6,T7,T8>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2,T3,T4,T5,T6,T7,T8> : QueryBase, ISource<T1,T2,T3,T4,T5,T6,T7,T8>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2,T3,T4,T5,T6,T7,T8> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6,T7,T8> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9> Join<T9>(System.Linq.Expressions.Expression<Func<T1,T2,T3,T4,T5,T6,T7,T8,T9, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2,T3,T4,T5,T6,T7,T8,T9>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2,T3,T4,T5,T6,T7,T8,T9> : QueryBase, ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> Join<T10>(System.Linq.Expressions.Expression<Func<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10, bool>> condition)
		{
			Context.AddQueryPart(new JoinPart(condition));
			return new Source<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>(Context);
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

	internal class Source<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> : QueryBase, ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>
	{
		public Source(QueryContext context)
			: base(context)
		{
		}

		public ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> Where(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10, bool>> condition)
		{
			Context.AddQueryPart(new WherePart(true, condition));
			return this;
		}
		public ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> GroupBy(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,object>> groupBy)
		{
			Context.AddQueryPart(new GroupByPart(groupBy));
			return this;
		}
		public IQueryEnd<TNew> Select<TNew>(System.Linq.Expressions.Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10, TNew>> select)
		{
			Context.AddQueryPart(new SelectPart(select));
			return new QueryEnd<TNew>(Context);
		}
	}

}
