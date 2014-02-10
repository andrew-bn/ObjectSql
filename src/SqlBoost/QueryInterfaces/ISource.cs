using System;
using System.Linq.Expressions;

namespace SqlBoost.QueryInterfaces
{
	public interface ISource<T1>
	{
		ISource<T1> Where(Expression<Func<ITargetDatabase, T1, bool>> condition);
		ISource<T1, T2> Join<T2>(Expression<Func<T1, T2, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,TSelect>> select);
		ISource<T1> GroupBy(Expression<Func<ITargetDatabase, T1,object>> groupBy);
	}
	public interface ISource<T1,T2>
	{
		ISource<T1,T2> Where(Expression<Func<ITargetDatabase, T1,T2, bool>> condition);
		ISource<T1,T2, T3> Join<T3>(Expression<Func<T1,T2, T3, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,TSelect>> select);
		ISource<T1,T2> GroupBy(Expression<Func<ITargetDatabase, T1,T2,object>> groupBy);
	}
	public interface ISource<T1,T2,T3>
	{
		ISource<T1,T2,T3> Where(Expression<Func<ITargetDatabase, T1,T2,T3, bool>> condition);
		ISource<T1,T2,T3, T4> Join<T4>(Expression<Func<T1,T2,T3, T4, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,T3,TSelect>> select);
		ISource<T1,T2,T3> GroupBy(Expression<Func<ITargetDatabase, T1,T2,T3,object>> groupBy);
	}
	public interface ISource<T1,T2,T3,T4>
	{
		ISource<T1,T2,T3,T4> Where(Expression<Func<ITargetDatabase, T1,T2,T3,T4, bool>> condition);
		ISource<T1,T2,T3,T4, T5> Join<T5>(Expression<Func<T1,T2,T3,T4, T5, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,T3,T4,TSelect>> select);
		ISource<T1,T2,T3,T4> GroupBy(Expression<Func<ITargetDatabase, T1,T2,T3,T4,object>> groupBy);
	}
	public interface ISource<T1,T2,T3,T4,T5>
	{
		ISource<T1,T2,T3,T4,T5> Where(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5, bool>> condition);
		ISource<T1,T2,T3,T4,T5, T6> Join<T6>(Expression<Func<T1,T2,T3,T4,T5, T6, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,TSelect>> select);
		ISource<T1,T2,T3,T4,T5> GroupBy(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,object>> groupBy);
	}
	public interface ISource<T1,T2,T3,T4,T5,T6>
	{
		ISource<T1,T2,T3,T4,T5,T6> Where(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6, bool>> condition);
		ISource<T1,T2,T3,T4,T5,T6, T7> Join<T7>(Expression<Func<T1,T2,T3,T4,T5,T6, T7, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,TSelect>> select);
		ISource<T1,T2,T3,T4,T5,T6> GroupBy(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,object>> groupBy);
	}
	public interface ISource<T1,T2,T3,T4,T5,T6,T7>
	{
		ISource<T1,T2,T3,T4,T5,T6,T7> Where(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7, bool>> condition);
		ISource<T1,T2,T3,T4,T5,T6,T7, T8> Join<T8>(Expression<Func<T1,T2,T3,T4,T5,T6,T7, T8, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,TSelect>> select);
		ISource<T1,T2,T3,T4,T5,T6,T7> GroupBy(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,object>> groupBy);
	}
	public interface ISource<T1,T2,T3,T4,T5,T6,T7,T8>
	{
		ISource<T1,T2,T3,T4,T5,T6,T7,T8> Where(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8, bool>> condition);
		ISource<T1,T2,T3,T4,T5,T6,T7,T8, T9> Join<T9>(Expression<Func<T1,T2,T3,T4,T5,T6,T7,T8, T9, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,TSelect>> select);
		ISource<T1,T2,T3,T4,T5,T6,T7,T8> GroupBy(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,object>> groupBy);
	}
	public interface ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9>
	{
		ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9> Where(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9, bool>> condition);
		ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9, T10> Join<T10>(Expression<Func<T1,T2,T3,T4,T5,T6,T7,T8,T9, T10, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,TSelect>> select);
		ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9> GroupBy(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,object>> groupBy);
	}
	public interface ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10>
	{
		ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> Where(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10, bool>> condition);
		IQueryEnd<TSelect> Select<TSelect>(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,TSelect>> select);
		ISource<T1,T2,T3,T4,T5,T6,T7,T8,T9,T10> GroupBy(Expression<Func<ITargetDatabase, T1,T2,T3,T4,T5,T6,T7,T8,T9,T10,object>> groupBy);
	}
}
