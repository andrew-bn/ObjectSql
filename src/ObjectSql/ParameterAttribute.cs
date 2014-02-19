using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql
{
	public class ParameterAttribute: Attribute
	{
		public string Name { get; private set; }
		public string TypeName { get; private set; }
		public ParameterDirection Direction { get; private set; }

		public ParameterAttribute(string name, string type)
		{
			Name = name;
			TypeName = type;
			Direction = ParameterDirection.Input;
		}

		public ParameterAttribute(string name, string type, ParameterDirection direction)
		{
			Name = name;
			TypeName = type;
			Direction = direction;
		}
	}
}
