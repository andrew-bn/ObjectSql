using System.Collections.Generic;

namespace ObjectSql.SqlServer.Schema
{
	public class Table: NameHolder
	{
		public Table()
		{
			Columns = new List<Column>();
		}

		public List<Column> Columns { get; set; }

		public override string ToString()
		{
			return string.Format("{0}.{1}", Schema,Name);
		}
	}
}