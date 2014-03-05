namespace ObjectSql.QueryInterfaces
{
	public interface IReturnValueHolder
	{
		TReturn MapReturnValue<TReturn>();
	}
}