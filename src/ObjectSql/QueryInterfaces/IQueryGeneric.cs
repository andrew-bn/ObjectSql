using System;
using System.Linq.Expressions;

namespace ObjectSql.QueryInterfaces
{
	public interface IQuery<TSource1>
	{
		IQuery<TSource1> Where(Expression<Func<TSource1, bool>> condition);
		IQuery<TSource1, TNewSource> Join<TNewSource>(Expression<Func<TSource1, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TEntity>> select);
		IQuery<TSource1> GroupBy(Expression<Func<TSource1,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2>
	{
		IQuery<TSource1,TSource2> Where(Expression<Func<TSource1,TSource2, bool>> condition);
		IQuery<TSource1,TSource2, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TEntity>> select);
		IQuery<TSource1,TSource2> GroupBy(Expression<Func<TSource1,TSource2,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3>
	{
		IQuery<TSource1,TSource2,TSource3> Where(Expression<Func<TSource1,TSource2,TSource3, bool>> condition);
		IQuery<TSource1,TSource2,TSource3, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3> GroupBy(Expression<Func<TSource1,TSource2,TSource3,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, bool>> condition);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, TNewSource> Join<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, TNewSource> LeftJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, TNewSource> RightJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, TNewSource, bool>> condition, params object[] additions);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, TNewSource> FullJoin<TNewSource>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14, TNewSource, bool>> condition, params object[] additions);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14,object>> groupBy);
	}
	public interface IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14,TSource15>
	{
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14,TSource15> Where(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14,TSource15, bool>> condition);
		IQueryEnd<TEntity> Select<TEntity>(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14,TSource15,TEntity>> select);
		IQuery<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14,TSource15> GroupBy(Expression<Func<TSource1,TSource2,TSource3,TSource4,TSource5,TSource6,TSource7,TSource8,TSource9,TSource10,TSource11,TSource12,TSource13,TSource14,TSource15,object>> groupBy);
	}
}
