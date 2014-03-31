using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql
{
	public class DatabaseTypesAttribute: Attribute
	{
		public string[] Types { get; private set; }

		public DatabaseTypesAttribute(params string[] types )
		{
			Types = types;
		}
	}
}
