using System.Collections.Generic;

namespace ObjectSql.SqlServer.Schema
{
	public enum RoutineType
	{
		Procedure,
		Function
	}
	public class Procedure
	{
		public Procedure()
		{
			Parameters = new List<Parameter>();
		}
		public string Schema { get; set; }
		public string Name { get; set; }
		public List<Parameter> Parameters { get; set; }
		public RoutineType RoutineType { get; set; }
		public override string ToString()
		{
			return string.Format("{0}.{1}", Schema, Name);
		}
	}
}