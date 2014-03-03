namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class ParametersSubstitutor<T>: IParameterSubstitutor
	{
		public T Table { get; set; }
		public string Name { get; set; }
	}
}