using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql
{
	public class ColumnAttribute: Attribute
	{
		public string Name { get; private set; }
		public string TypeName { get; set; }
		public ColumnAttribute(string name)
		{
			Name = name;
			TypeName = "";
		}
		public ColumnAttribute(string name, string typeName)
		{
			Name = name;
			TypeName = typeName;
		}
	}
}
