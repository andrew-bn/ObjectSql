namespace ObjectSql.QueryInterfaces
{
	public interface INonQueryResultHolder:IReturnValueHolder
	{
		int NonQueryResult { get; }
	}
}