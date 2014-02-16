namespace ObjectSql.QueryInterfaces
{
	public interface IStoredProcedureHolder<T>
	{
		ISql Root { get; }
	}
}