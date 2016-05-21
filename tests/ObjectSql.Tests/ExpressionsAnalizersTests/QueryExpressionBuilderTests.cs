using NUnit.Framework;
using System.Data;

namespace ObjectSql.Tests.ExpressionsAnalizersTests
{
	[TestFixture]
	public class QueryExpressionBuilderTests : TestBase
	{
		[Test]
		public void Constant()
		{
			EfQuery.Select(()=>5)
				 .Verify("SELECT @p0",5.DbType(SqlDbType.Int));
		}
		[Test]
		public void Constant_Complex()
		{
			EfQuery.Select(() => 5)
				 .Verify("SELECT @p0", 5.DbType(SqlDbType.Int));
			EfQuery.Select(() => 6)
				.Verify("SELECT @p0", 6.DbType(SqlDbType.Int));
		}
		[Test]
		public void ClassProperty()
		{
			var val = 5;
			EfQuery.Select(() => val)
				 .Verify("SELECT @p0", val.DbType(SqlDbType.Int));
		}
		[Test]
		public void ClassProperty_Complex()
		{
			var val = 5;
			EfQuery.Select(() => val)
				 .Verify("SELECT @p0", val.DbType(SqlDbType.Int));

			val = 6;
			EfQuery.Select(() => val)
				 .Verify("SELECT @p0", val.DbType(SqlDbType.Int));
		}
		[Test]
		public void ClassNestedProperty()
		{
			var val = new{val = 5};
			EfQuery.Select(() => val.val)
				 .Verify("SELECT @p0", val.val.DbType(SqlDbType.Int));
		}
		[Test]
		public void ClassNestedProperty_Complex()
		{
			var val = new { val = 5 };
			EfQuery.Select(() => val.val)
				 .Verify("SELECT @p0", val.val.DbType(SqlDbType.Int));

			val = new { val = 10 };
			EfQuery.Select(() => val.val)
				 .Verify("SELECT @p0", val.val.DbType(SqlDbType.Int));
		}
		[Test]
		public void TwoConstants()
		{
			var val = new { val = 5 };
			EfQuery.Select(() => val.val+"25")
				 .Verify("SELECT @p0", "525".DbType(SqlDbType.NVarChar));
		}

		[Test]
		public void TwoConstants_Complex()
		{
			var val = new { val = 5 };
			EfQuery.Select(() => val.val + "25")
				 .Verify("SELECT @p0", "525".DbType(SqlDbType.NVarChar));

			val = new { val = 6 };
			EfQuery.Select(() => val.val + "25")
				 .Verify("SELECT @p0", "625".DbType(SqlDbType.NVarChar));
		}
		[Test]
		public void ThreeConstants()
		{
			EfQuery.Select(() => "val"+2+"val")
				 .Verify("SELECT @p0", "val2val".DbType(SqlDbType.NVarChar));
		}
		[Test]
		public void ThreeConstants_Complex()
		{
			EfQuery.Select(() => "val" + 2 + "val")
				 .Verify("SELECT @p0", "val2val".DbType(SqlDbType.NVarChar));

			EfQuery.Select(() => "val" + 3 + "val")
	 .Verify("SELECT @p0", "val3val".DbType(SqlDbType.NVarChar));
		}
		//	[Test]
		//	public void BuildSql_RenderBinarySub()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID - 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]-@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderDivide()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID / 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]/@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderMult()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID * 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]*@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderGreater()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID > 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]>@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderGreaterOrEqual()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID >= 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]>=@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderLess()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID < 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]<@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderLessOrEqual()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID <= 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]<=@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderNotEqual()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID != 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]<>@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	public enum Foo
		//	{
		//		Val1,
		//		Val2
		//	}
		//	[Test]
		//	public void BuildSql_RenderEqualToEnum()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID == (int)Foo.Val2;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot((int)Foo.Val2);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]=@p0)", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => (int)Foo.Val2), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderEqualToEnumVariable()
		//	{
		//		var enumVar = (Foo)Enum.Parse(typeof(Foo), "Val2");
		//		AddQueryRoot(() => enumVar);
		//		Expression<Func<Category, object>> exp = (c) => c.CategoryID == (int)enumVar;
		//		var builder = CreateBuilder();
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[CategoryID]=@p0)", result);
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderNull()
		//	{
		//		Expression<Func<object>> exp = () => null;
		//		var builder = CreateBuilder();
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("NULL", result);
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		//	}
		//	[Test]
		//	public void BuildSql_RenderIsNull()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.Picture == null;
		//		var builder = CreateBuilder();
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[Picture]ISNULL)", result);
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		//	}
		//	[Test]
		//	public void BuildSql_RenderIsNotNull()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.Picture != null;
		//		var builder = CreateBuilder();
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[Picture]ISNOTNULL)", result);
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		//	}
		//	[Test]
		//	public void BuildSql_RenderAnd()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.Description == null && c.CategoryID == 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("(([c].[Description]ISNULL)AND([c].[CategoryID]=@p0))", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderOr()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.Description == null || c.CategoryID == 1;
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("(([c].[Description]ISNULL)OR([c].[CategoryID]=@p0))", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	[Test]
		//	public void BuildSql_RenderNot()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => !(c.CategoryID == 1);
		//		var builder = CreateBuilder();
		//		QueryRoots.AddRoot(1);
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("(NOT([c].[CategoryID]=@p0))", result);
		//		_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		//	}
		//	public int GetConstant()
		//	{
		//		return 0;
		//	}
		//	[Test]
		//	public void BuildSql_MethodCall_ErrorNotExpected()
		//	{
		//		Expression<Func<object>> exp = () => this.GetConstant();
		//		var builder = CreateBuilder();
		//		builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();
		//	}

		//	protected Expression IsExp<T>(Expression<Func<T>> b)
		//	{
		//		return It.Is<Expression>(e => e.AreEqual(b));
		//	}

		//	private QueryExpressionBuilder CreateBuilder()
		//	{
		//		return new QueryExpressionBuilder(_schemaManager.Object, _delegatesBuilder.Object, SqlServerSqlWriter.Instance);
		//	}
		//}
		//[DatabaseExtension]
		//public class TestExtension
		//{
		//	public static bool LikeForTests(string left, string right)
		//	{
		//		return false;
		//	}
		//	public static bool RenderLikeWasCalled = false;
		//	internal static string RenderLikeForTests(BuilderContext commandPreparators, string[] parts)
		//	{
		//		RenderLikeWasCalled = true;
		//		return string.Format("({0} LIKE {1})", parts[0], parts[1]);
		//	}
	}
}
