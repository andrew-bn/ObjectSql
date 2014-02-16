namespace ObjectSql.QueryInterfaces
{
	public interface IEmit<T>
	{
		T Emit(string sql);
	}
}
