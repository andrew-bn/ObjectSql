namespace ObjectSql.QueryInterfaces
{
	public interface IScalarResultHolder: IReturnValueHolder
	{
		object ScalarResult { get; }
	}
}