using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql
{
	public class EmitAttribute: Attribute
	{
		public string Value { get; private set; }

		public EmitAttribute(string value)
		{
			Value = value;
		}
	}
}
