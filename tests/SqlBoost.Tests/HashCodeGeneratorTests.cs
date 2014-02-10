using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using SqlBoost.Core;
using System.Linq;
using System.Collections.Generic;
using SqlBoost.Core.Bo;
using SqlBoost.Core.Misc;
namespace SqlBoost.Tests
{
	[TestClass]
	public class HashCodeGeneratorTests
	{
		public class Foo
		{
			public string Param1 { get; set; }
			public Foo() { }
			public Foo(int i) { }
			public int Method()
			{
				return 0;
			}
			public bool Param3 { get; set; }
			public Foo FooParam { get; set; }
		}
		[TestMethod]
		public void DifferentRoots()
		{
			const bool constantClosureSource = true;
			string closureSource = "closureSource";
			Expression<Func<Foo, object>> exp =
				f => f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 12 &&
					f.FooParam.Method() == f.FooParam.FooParam.Method();

			var p = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp, ref p);

			Assert.AreEqual(1, p.Roots.Count);
			var pairs = p.Roots.ToArray();

			dynamic firstVal = pairs[0].Key;

			Assert.AreEqual(closureSource, firstVal.closureSource);
			Assert.AreEqual(1, pairs[0].Value);

		}
		[TestMethod]
		public void DifferentRoots_OneRootPropertyValueIsNull()
		{
			const bool constantClosureSource = true;
			string closureSource = null;
			Expression<Func<Foo, object>> exp =
				f => f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 12 &&
					f.FooParam.Method() == f.FooParam.FooParam.Method();

			var p = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp, ref p);

			Assert.AreEqual(1, p.Roots.Count);
			var pairs = p.Roots.ToArray();

			dynamic firstVal = pairs[0].Key;

			Assert.AreEqual(closureSource, firstVal.closureSource);
			Assert.AreEqual(1, pairs[0].Value);

		}
		[TestMethod]
		public void NullRoots()
		{
			Expression<Func<Foo, object>> exp =
				f => f.FooParam.Param1 == null &&
					f.Method() == 12 &&
					f.FooParam == null;

			var p = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp, ref p);

			Assert.AreEqual(0, p.Roots.Count);
		}
		[TestMethod]
		public void DuplicatedRoot()
		{
			const bool constantClosureSource = true;
			var closureSource = "closureSource";
			var closureSource2 = 4;
			Expression<Func<Foo, object>> exp =
				f => f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 12 &&
					closureSource2 == f.FooParam.FooParam.Method();

			var p = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp,ref p);

			Assert.AreEqual(1, p.Roots.Count);
			var pairs = p.Roots.ToArray();


			dynamic firstVal = pairs[0].Key;

			Assert.AreEqual(closureSource, firstVal.closureSource);
			Assert.AreEqual(closureSource2, firstVal.closureSource2);
			Assert.AreEqual(1 | (1<<1), pairs[0].Value);
		}

		public int Value = 4;
		[TestMethod]
		public void DifferentRoots3()
		{
			const bool constantClosureSource = true;
			var closureSource = "closureSource";
			Expression<Func<Foo, object>> exp =
				f => f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 12 &&
					Value == f.FooParam.FooParam.Method();

			var p = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp, ref p);

			Assert.AreEqual(2, p.Roots.Count);
			var pairs = p.Roots.ToArray();

			dynamic firstVal = pairs[0].Key;
			dynamic secondVal = pairs[1].Key;
			Assert.AreEqual(closureSource, firstVal.closureSource);
			Assert.AreEqual(Value, secondVal.Value);
			Assert.AreEqual(1, pairs[0].Value);
			Assert.AreEqual(2, pairs[1].Value);
		}
		[TestMethod]
		public void DifferentHashCodes_ParameterReplacement()
		{
			const bool constantClosureSource = true;
			var closureSource = "closureSource";
			var closureSource2 = 4;
			Expression<Func<Foo, object>> exp1 =
				f => f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					f.Method() == 12 &&
					closureSource2 == f.FooParam.FooParam.Method();
			Expression<Func<Foo, object>> exp2 =
				f => f.FooParam.Param1 == closureSource &&
					f.Param3 == constantClosureSource ||
					12 == f.Method() &&
					closureSource2 == f.FooParam.FooParam.Method();

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1,ref p1);
			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2,ref p2);

			Assert.IsFalse(p1.Hash == p2.Hash);
		}
		[TestMethod]
		public void DifferentRootParametersValues_SameHashCodes()
		{
			var closureSource2 = 4;
			Expression<Func<Foo, object>> exp1 =
				f => closureSource2 == f.FooParam.FooParam.Method();

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			closureSource2 = 234;
			Expression<Func<Foo, object>> exp2 =
				f => closureSource2 == f.FooParam.FooParam.Method();


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.IsTrue(p1.Hash == p2.Hash);
		}
		[TestMethod]
		public void DifferentConstantValues_DifferentHashCodes()
		{
			Expression<Func<Foo, object>> exp1 =
				f => 5 == f.FooParam.FooParam.Method();

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			Expression<Func<Foo, object>> exp2 =
				f => 6 == f.FooParam.FooParam.Method();


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.IsFalse(p1.Hash == p2.Hash);
		}
		[TestMethod]
		public void SameConstantValues_SameHashCodes()
		{
			Expression<Func<Foo, object>> exp1 =
				f => 5 == f.FooParam.FooParam.Method();

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			Expression<Func<Foo, object>> exp2 =
				f => 5 == f.FooParam.FooParam.Method();


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.IsTrue(p1.Hash == p2.Hash);
		}

		[TestMethod]
		public void ComplexCase_ConstantsHasSameValues_RootsPropertiesHasDifferentValues()
		{
			const bool constantClosureSource = true;
			var closureSource = "closureSource";
			var closureSource2 = 4;
			var val = 23;
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
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.IsTrue(p1.Hash == p2.Hash);
		}

		[TestMethod]
		public void ComplexCase_ConstantsHasDifferentValues_RootsPropertiesHasSameValues()
		{
			const bool constantClosureSource = true;
			var closureSource = "closureSource";
			var closureSource2 = 4;
			var val = 23;
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
			const bool newConstantClosureSource = false;
			Expression<Func<Foo, object>> exp2 =
				f => new
				{
					P1 = f.FooParam.Method(),
					P2 = 3448,
					P3 = new[] { 1, val, f.Method() },
					P4 = new { f.Param1 },
					P5 = new Foo(),
					P6 = new Foo(1),
					P7 = new Foo() { Param1 = "param1", Param3 = !f.Param3, FooParam = null },
					P8 = f.FooParam.Param1 == closureSource &&
					f.Param3 == newConstantClosureSource ||
					f.Method() == 132 &&
					closureSource2 == f.FooParam.FooParam.Method()
				};

			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2,ref p2);

			Assert.IsFalse(p1.Hash == p2.Hash);
		}
	}
}

