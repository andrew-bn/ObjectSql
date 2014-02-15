using System.Collections.Generic;

namespace SqlBoost.SqlServer.Schema
{
	public class Table
	{
		public Table()
		{
			Columns = new List<Column>();
		}

		public string Schema { get; set; }
		public string Name { get; set; }
		public List<Column> Columns { get; set; }

		public override string ToString()
		{
			return string.Format("{0}.{1}", Schema,Name);
		}
	}
}