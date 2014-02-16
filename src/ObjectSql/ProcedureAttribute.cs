using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql
{
	public class ProcedureAttribute : Attribute
	{
		public string Name { get; private set; }
		public string Schema { get; private set; }

		public ProcedureAttribute(string name, string schema)
		{
			Name = name;
			Schema = schema;
		}
	}
}
