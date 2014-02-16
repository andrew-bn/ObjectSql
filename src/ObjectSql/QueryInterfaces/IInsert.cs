namespace ObjectSql.QueryInterfaces
{
	public interface IInsert<TDst>
		where TDst:class
	{
		INonQueryEnd Values(params TDst[] values);
	}
}
