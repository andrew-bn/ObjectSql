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
		[TestMethod]
		[Ignore]
		public void BooleanExpressionEnumerationFlowIsValid()
		{
			int val = 1;
			Expression<Func<Foo, bool>> exp =
				f => f.Param1 == "param1" &&
					 f.Method() == val &&
					 1 != f.GetType().GetHashCode();

			var sb = new StringBuilder();
			foreach (var item in ExpressionEnumerator.Enumerate(exp))
			{
				sb.AppendLine(item.ToString());
			}
			var result = sb.ToString();
			Assert.AreEqual(
@"f => (((f.Param1 == ""param1"") AndAlso (f.Method() == value(SqlBoostTests.ExpressionEnumeratorTests+<>c__DisplayClass0).val)) AndAlso (1 != f.GetType().GetHashCode()))
(((f.Param1 == ""param1"") AndAlso (f.Method() == value(SqlBoostTests.ExpressionEnumeratorTests+<>c__DisplayClass0).val)) AndAlso (1 != f.GetType().GetHashCode()))
((f.Param1 == ""param1"") AndAlso (f.Method() == value(SqlBoostTests.ExpressionEnumeratorTests+<>c__DisplayClass0).val))
(f.Param1 == ""param1"")
f.Param1
f
""param1""
(f.Method() == value(ObjectSqlTests.ExpressionEnumeratorTests+<>c__DisplayClass0).val)
f.Method()
f
value(ObjectSqlTests.ExpressionEnumeratorTests+<>c__DisplayClass0).val
value(ObjectSqlTests.ExpressionEnumeratorTests+<>c__DisplayClass0)
(1 != f.GetType().GetHashCode())
1
f.GetType().GetHashCode()
f.GetType()
f
f
", result);
		}

		[TestMethod]
		[Ignore]
		public void NewExpressionEnumerationFlowIsValid()
		{
			int val = 1;
			Expression<Func<Foo, object>> exp =
				f => new
				{
					P1 = f.FooParam.Method(),
					P2 = 12,
					P3 = new[] { 1, val, f.Method() },
					P4 = new { f.Param1 },
					P5 = new Foo(),
					P6 = new Foo(1),
					P7 = new Foo() { Param1 = "param1", Param3 = !f.Param3, FooParam = null }
				};

			var sb = new StringBuilder();
			foreach (var item in ExpressionEnumerator.Enumerate(exp))
			{
				sb.AppendLine(item.ToString());
			}
			var result = sb.ToString();
			Assert.AreEqual(
@"f => new <>f__AnonymousType7`7(P1 = f.FooParam.Method(), P2 = 12, P3 = new [] {1, value(ObjectSqlTests.ExpressionEnumeratorTests+<>c__DisplayClass2).val, f.Method()}, P4 = new <>f__AnonymousType5`1(Param1 = f.Param1), P5 = new Foo(), P6 = new Foo(1), P7 = new Foo() {Param1 = ""param1"", Param3 = Not(f.Param3), FooParam = null})
new <>f__AnonymousType7`7(P1 = f.FooParam.Method(), P2 = 12, P3 = new [] {1, value(ObjectSqlTests.ExpressionEnumeratorTests+<>c__DisplayClass2).val, f.Method()}, P4 = new <>f__AnonymousType5`1(Param1 = f.Param1), P5 = new Foo(), P6 = new Foo(1), P7 = new Foo() {Param1 = ""param1"", Param3 = Not(f.Param3), FooParam = null})
f.FooParam.Method()
f.FooParam
f
12
new [] {1, value(ObjectSqlTests.ExpressionEnumeratorTests+<>c__DisplayClass2).val, f.Method()}
1
value(ObjectSqlTests.ExpressionEnumeratorTests+<>c__DisplayClass2).val
value(ObjectSqlTests.ExpressionEnumeratorTests+<>c__DisplayClass2)
f.Method()
f
new <>f__AnonymousType5`1(Param1 = f.Param1)
f.Param1
f
new Foo()
new Foo(1)
1
new Foo() {Param1 = ""param1"", Param3 = Not(f.Param3), FooParam = null}
new Foo()
Param1 = ""param1""
""param1""
Param3 = Not(f.Param3)
Not(f.Param3)
f.Param3
f
FooParam = null
null
f
", result);
		}

	}
}
