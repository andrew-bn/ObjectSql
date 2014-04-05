using NUnit.Framework;
using ObjectSql.Core.Bo;
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
	public class ExpressionComparerTests
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
		[Test]
		public void ExpressionsAreEqual_ConstantsHasSameValues()
		{
			const bool constantClosureSource = true;
			var closureSource = "closureSource";
			var closureSource2 = 4;
			var val = 222;
			Expression<Func<Foo, object>> exp1 =
				f => new
				{
					P1 = f.FooParam.Method(),
					P2 = 348,
					P3 = new[] { 1, val, f.Method() },
					P4 = new { f.Param1 },
					P5 = new Foo(),
					P6 = new Foo(1),
					P7 = new Foo() { Param1 = "param1", Param3 = !f.Param3, FooParam = null },
					P8 = f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 12 &&
					closureSource2 == f.FooParam.FooParam.Method()
				};

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1,ref p1);

			closureSource = "new value of closureSource";
			closureSource2 = 234;
			Expression<Func<Foo, object>> exp2 =
				f => new
				{
					P1 = f.FooParam.Method(),
					P2 = 348,
					P3 = new[] { 1, val, f.Method() },
					P4 = new { f.Param1 },
					P5 = new Foo(),
					P6 = new Foo(1),
					P7 = new Foo() { Param1 = "param1", Param3 = !f.Param3, FooParam = null },
					P8 = f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 12 &&
					closureSource2 == f.FooParam.FooParam.Method()
				};


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2,ref p2);

			var result = ExpressionComparer.AreEqual(exp1,ref p1, exp2,ref p2);

			Assert.IsTrue(result);
		}
		[Test]
		public void ExpressionsAreEqual_RootsHaveSameValues()
		{
			const bool constantClosureSource = true;
			var closureSource = "closureSource";
			var closureSource2 = 4;
			var val = 222;
			Expression<Func<Foo, object>> exp1 =
				f => new
				{
					P1 = f.FooParam.Method(),
					P2 = 348,
					P3 = new[] { 1, val, f.Method() },
					P4 = new { f.Param1 },
					P5 = new Foo(),
					P6 = new Foo(1),
					P7 = new Foo() { Param1 = "param1", Param3 = !f.Param3, FooParam = null },
					P8 = f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 12 &&
					closureSource2 == f.FooParam.FooParam.Method()
				};

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			Expression<Func<Foo, object>> exp2 =
				f => new
				{
					P1 = f.FooParam.Method(),
					P2 = 348,
					P3 = new[] { 1, val, f.Method() },
					P4 = new { f.Param1 },
					P5 = new Foo(),
					P6 = new Foo(1),
					P7 = new Foo() { Param1 = "param2", Param3 = !f.Param3, FooParam = null },
					P8 = f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 132 &&
					closureSource2 == f.FooParam.FooParam.Method()
				};


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			var result = ExpressionComparer.AreEqual(exp1, ref p1, exp2, ref p2);

			Assert.IsTrue(result);
		}
	}
}
