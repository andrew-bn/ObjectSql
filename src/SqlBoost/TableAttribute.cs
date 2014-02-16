using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost
{
	public class TableAttribute: Attribute
	{
		public string Name { get; private set; }
		public string Schema { get; set; }

		public TableAttribute(string name)
		{
			Name = name;
			Schema = "";
		}
		public TableAttribute (string name, string schema)
		{
			Name = name;
			Schema = schema;
		}
	}
}
