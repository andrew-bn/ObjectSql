using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	[DatabaseExtension]
	public class ParametersSubstitutor<T>: IParameterSubstitutor
	{
		public T Table { get; set; }
		public string Name { get; set; }
	}
}