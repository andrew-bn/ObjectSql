namespace ObjectSql.QueryInterfaces
{
	public interface IScalarEnd: ISqlEnd
	{
		object ExecuteScalar();
	}
}
