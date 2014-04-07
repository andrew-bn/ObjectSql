using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql
{
	public class SqlAttribute:Attribute
	{
		public string Pattern { get; private set; }

		public SqlAttribute(string pattern)
		{
			Pattern = pattern;
		}
	}
}
