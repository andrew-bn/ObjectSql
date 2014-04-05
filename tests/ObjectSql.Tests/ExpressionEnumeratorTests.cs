using NUnit.Framework;
using ObjectSql.Core.Misc;
using ObjectSql.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Tests
{
	[TestFixture]
	public class ExpressionEnumeratorTests
	{
		public class Foo
		{
			public Foo() { }
			public Foo(int param) { }
			public string Param1 { get; set; }
			public int Method()
			{
				return 0;
			}
			public bool Param3 { get; set; }
			public Foo FooParam { get; set; }
		}
	}
}
