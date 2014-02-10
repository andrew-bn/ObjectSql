namespace SqlBoost.QueryInterfaces
{
	public interface IScalarEnd: ISqlEnd
	{
		object ExecuteScalar();
	}
}
