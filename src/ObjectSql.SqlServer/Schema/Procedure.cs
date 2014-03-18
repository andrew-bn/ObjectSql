using System.Collections.Generic;

namespace ObjectSql.SqlServer.Schema
{
	public class Procedure: NameHolder
	{
		public Procedure()
		{
			Parameters = new List<Parameter>();
			UseSchema = false;
		}
		
		public List<Parameter> Parameters { get; set; }
		public RoutineType RoutineType { get; set; }

		public override string ToString()
		{
			return string.Format("{0}.{1}", Schema, Name);
		}
	}
}