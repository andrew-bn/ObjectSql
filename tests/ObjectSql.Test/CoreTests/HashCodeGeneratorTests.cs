using System;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Misc;
using System.Linq;
using Xunit;

namespace ObjectSql.Tests
{
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
		[Fact]
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

			Assert.Equal(3, p.Roots.Count);
			var pairs = p.Roots.ToArray();

			dynamic firstVal = pairs[0];
			Assert.Equal(closureSource, firstVal.closureSource);
			Assert.Equal(constantClosureSource, pairs[1]);
			Assert.Equal(12, pairs[2]);

		}
		[Fact]
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

			Assert.Equal(3, p.Roots.Count);
			var pairs = p.Roots.ToArray();

			dynamic firstVal = pairs[0];

			Assert.Equal(closureSource, firstVal.closureSource);
			Assert.Equal(constantClosureSource, pairs[1]);
			Assert.Equal(12, pairs[2]);

		}
		[Fact]
		public void NullRoots()
		{
			Expression<Func<Foo, object>> exp =
				f => f.FooParam.Param1 == null &&
					f.Method() == 12 &&
					f.FooParam == null;

			var p = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp, ref p);

			Assert.Equal(1, p.Roots.Count);
			Assert.Equal(12, p.Roots.ToArray()[0]);
		}
		[Fact]
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
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp, ref p);

			Assert.Equal(3, p.Roots.Count);
			var pairs = p.Roots.ToArray();


			dynamic firstVal = pairs[0];

			Assert.Equal(closureSource, firstVal.closureSource);
			Assert.Equal(closureSource2, firstVal.closureSource2);
			Assert.Equal(constantClosureSource, pairs[1]);
			Assert.Equal(12, pairs[2]);
		}

		public int Value = 4;
		[Fact]
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

			Assert.Equal(4, p.Roots.Count);
			var pairs = p.Roots.ToArray();

			dynamic firstVal = pairs[0];
			dynamic secondVal = pairs[3];
			Assert.Equal(closureSource, firstVal.closureSource);
			Assert.Equal(constantClosureSource, pairs[1]);
			Assert.Equal(12, pairs[2]);
			Assert.Equal(Value, secondVal.Value);
		}
		[Fact]
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
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);
			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.False(p1.Hash == p2.Hash);
		}
		[Fact]
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

			Assert.True(p1.Hash == p2.Hash);
		}
		[Fact]
		public void DifferentRoots_SameHashCodes()
		{
			var closureSource2 = 4;
			var exp1 = GetExpressionTree(closureSource2);

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			closureSource2 = 234;
			var exp2 = GetExpressionTree(closureSource2);


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.IsTrue(p1.Hash == p2.Hash);
		}

		private Expression<Func<Foo, object>> GetExpressionTree(int value)
		{
			
			Expression<Func<Foo, object>> exp =
				f => value == f.FooParam.FooParam.Method() && value != 4;
			return exp;
		}
		[Fact]
		public void DifferentRoots_SameHashCodes_ComplexClosure()
		{
			var closureSource2 = 4;
			var strSrc = "asdf";
			var exp1 = GetExpressionTree4(closureSource2, strSrc);

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			closureSource2 = 234;
			strSrc = "asddf";
			var exp2 = GetExpressionTree4(closureSource2, strSrc);


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.IsTrue(p1.Hash == p2.Hash);
		}

		private Expression<Func<Foo, object>> GetExpressionTree4(int value, string value2)
		{

			Expression<Func<Foo, object>> exp =
				f => value == f.FooParam.FooParam.Method() && value != 4 && f.Param1 != value2;
			return exp;
		}
		[Fact]
		public void DifferentRoots_DiffHashCodes_ComplexClosure()
		{
			var closureSource2 = 4;
			var strSrc = "asdf";
			var exp1 = GetExpressionTree4(closureSource2, strSrc);

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			closureSource2 = 234;
			strSrc = "asddf";
			var exp2 = GetExpressionTree5(closureSource2, strSrc);


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.IsTrue(p1.Hash != p2.Hash);
		}

		private Expression<Func<Foo, object>> GetExpressionTree5(int value, string value2)
		{

			Expression<Func<Foo, object>> exp =
				f => value == f.FooParam.FooParam.Method() && f.Param1 != value2 && value != 4;
			return exp;
		}
		[Fact]
		public void DifferentRoots_DifferentHashCodes()
		{
			var closureSource2 = 4;
			var exp1 = GetExpressionTree2(closureSource2);

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			closureSource2 = 234;
			var exp2 = GetExpressionTree3(closureSource2);


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.IsTrue(p1.Hash != p2.Hash);
		}

		private Expression<Func<Foo, object>> GetExpressionTree2(int value)
		{

			Expression<Func<Foo, object>> exp =
				f => value == f.FooParam.FooParam.Method() && value != 4;
			return exp;
		}
		private Expression<Func<Foo, object>> GetExpressionTree3(int value)
		{

			Expression<Func<Foo, object>> exp =
				f => value != 4 && value == f.FooParam.FooParam.Method();
			return exp;
		}
		[Fact]
		public void DifferentConstantValues_SameHashCodes()
		{
			Expression<Func<Foo, object>> exp1 =
				f => 5 == f.FooParam.FooParam.Method();

			var p1 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);

			Expression<Func<Foo, object>> exp2 =
				f => 6 == f.FooParam.FooParam.Method();


			var p2 = new QueryRoots();
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.True(p1.Hash == p2.Hash);
		}
		[Fact]
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

			Assert.True(p1.Hash == p2.Hash);
		}

		[Fact]
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

			Assert.True(p1.Hash == p2.Hash);
		}

		[Fact]
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
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp1, ref p1);
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
			ExpressionHashCalculator.CalculateHashAndExtractConstantRoots(exp2, ref p2);

			Assert.True(p1.Hash == p2.Hash);
		}
	}
}

