using System.Collections.Generic;

namespace SqlBoost.SqlServer.Schema
{
	public class Procedure
	{
		public Procedure()
		{
			Parameters = new List<Parameter>();
		}
		public string Schema { get; set; }
		public string Name { get; set; }
		public List<Parameter> Parameters { get; set; }
		public override string ToString()
		{
			return string.Format("{0}.{1}", Schema, Name);
		}
	}
}