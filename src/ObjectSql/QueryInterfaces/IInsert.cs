namespace ObjectSql.QueryInterfaces
{
	public interface IInsert<TSource>
		where TSource:class
	{
		IQueryEnd Values(params TSource[] values);
	}
}
