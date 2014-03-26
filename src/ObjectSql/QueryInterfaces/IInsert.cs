namespace ObjectSql.QueryInterfaces
{
	public interface IInsert<TSource> : IQueryContextHolder
		where TSource:class
	{
		IQueryEnd Values(params TSource[] values);
	}
}
