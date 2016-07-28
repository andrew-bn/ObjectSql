using System;
using System.Data;
using ObjectSql.Test;
using Xunit;

namespace ObjectSql.Tests.ExpressionsAnalizersTests
{
	public class QueryExpressionBuilderTests : TestBase
	{
		[Fact]
		public void Constant()
		{
			Query.Select(() => 5)
				 .Verify("SELECT @p0", 5.DbType(SqlDbType.Int));
		}

		[Fact]
		public void ClassProperty()
		{
			var val = 5;
			Query.Select(() => val)
				 .Verify("SELECT @p0", val.DbType(SqlDbType.Int));
		}
		[Fact]
		public void ClassNestedProperty()
		{
			var val = new { val = 5 };
			Query.Select(() => val.val)
				 .Verify("SELECT @p0", val.val.DbType(SqlDbType.Int));
		}
		[Fact]
		public void TwoConstants()
		{
			var val = new { val = 5 };
			Query.Select(() => val.val + "25")
				 .Verify("SELECT @p0", "525".DbType(SqlDbType.NVarChar));
		}
		[Fact]
		public void ThreeConstants()
		{
			Query.Select(() => "val" + 2 + "val")
				 .Verify("SELECT @p0", "val2val".DbType(SqlDbType.NVarChar));
		}

		[Fact]
		public void Should_Create_Valid_Sql()
		{
			var userName = "user";
			var data = new { Password = "pwd"};
			var data2 = new {Id = 234};
			Query.From<Mailboxes>()
				.Where(m => m.Username == userName && m.Locked == true && m.ManualLock == true &&
				            m.UserDisabled == false && m.Password == data.Password && m.SubscriptionsId == data2.Id)
				.Select(m => m)
				.Verify("SELECT [m].[Id], [m].[Username], [m].[Password], [m].[Name], [m].[Quota], [m].[Domain], [m].[SubscriptionsId], " +
							"[m].[Created], [m].[Modified], [m].[LocalPart], [m].[UserDisabled], [m].[Locked], [m].[Pending], [m].[UsedBytes]," +
							" [m].[LastUpdateUsedBytesTime], [m].[ActualFeatureSetVersion], [m].[ManualLock], [m].[UserLocked], " +
							"[m].[AdminLocked], [m].[AutoLocked] " +
							"FROM [mail].[Mailboxes] AS [m] " +
							"WHERE  ( ( ( ( ( ([m].[Username] = @p0) AND  ([m].[Locked] = @p1)) " +
							"AND  ([m].[ManualLock] = @p1)) " +
							"AND  ([m].[UserDisabled] = @p2)) " +
							"AND  ([m].[Password] = @p3)) " +
							"AND  ([m].[SubscriptionsId] = @p4))",
				  userName.DbType(SqlDbType.NVarChar),
				  true.DbType(SqlDbType.Bit),
				  false.DbType(SqlDbType.Bit),
				  data.Password.DbType(SqlDbType.NVarChar),
				  data2.Id.DbType(SqlDbType.Int));

		}

		[Fact]
		public void Should_Create_Valid_Cache()
		{
			var data = new {Username = "user", Password = "pwd"};
			var data2 = new { Id = 234};
			Query.From<Mailboxes>()
					.Where(m => m.Username == data.Username && m.Locked == true && m.ManualLock == true &&
					m.UserDisabled == false && m.Password == data.Password && m.SubscriptionsId == data2.Id)
					.Select(m=>m)
					.Verify("SELECT [m].[Id], [m].[Username], [m].[Password], [m].[Name], [m].[Quota], [m].[Domain], [m].[SubscriptionsId], " +
					        "[m].[Created], [m].[Modified], [m].[LocalPart], [m].[UserDisabled], [m].[Locked], [m].[Pending], [m].[UsedBytes]," +
					        " [m].[LastUpdateUsedBytesTime], [m].[ActualFeatureSetVersion], [m].[ManualLock], [m].[UserLocked], " +
					        "[m].[AdminLocked], [m].[AutoLocked] " +
					        "FROM [mail].[Mailboxes] AS [m] " +
					        "WHERE  ( ( ( ( ( ([m].[Username] = @p0) AND  ([m].[Locked] = @p1)) " +
					        "AND  ([m].[ManualLock] = @p1)) " +
					        "AND  ([m].[UserDisabled] = @p2)) " +
					        "AND  ([m].[Password] = @p3)) " +
					        "AND  ([m].[SubscriptionsId] = @p4))",

				  data.Username.DbType(SqlDbType.NVarChar),
				  true.DbType(SqlDbType.Bit),
				  false.DbType(SqlDbType.Bit),
				  data.Password.DbType(SqlDbType.NVarChar),
				  data2.Id.DbType(SqlDbType.Int));

			Query.From<Mailboxes>()
					.Where(m => m.Username == data.Username && m.Locked == true && m.ManualLock == false &&
					m.UserDisabled == false && m.Password == data.Password && m.SubscriptionsId == data2.Id)
					.Select(m => m)
					.Verify("SELECT [m].[Id], [m].[Username], [m].[Password], [m].[Name], [m].[Quota], [m].[Domain], [m].[SubscriptionsId], " +
					        "[m].[Created], [m].[Modified], [m].[LocalPart], [m].[UserDisabled], [m].[Locked], [m].[Pending]," +
					        " [m].[UsedBytes], [m].[LastUpdateUsedBytesTime], [m].[ActualFeatureSetVersion], [m].[ManualLock], " +
					        "[m].[UserLocked], [m].[AdminLocked], [m].[AutoLocked] " +
					        "FROM [mail].[Mailboxes] AS [m] " +
					        "WHERE  ( ( ( ( ( ([m].[Username] = @p0) AND  ([m].[Locked] = @p1)) " +
					        "AND  ([m].[ManualLock] = @p2)) " +
					        "AND  ([m].[UserDisabled] = @p2)) " +
					        "AND  ([m].[Password] = @p3)) " +
					        "AND  ([m].[SubscriptionsId] = @p4))",
				  data.Username.DbType(SqlDbType.NVarChar),
				  true.DbType(SqlDbType.Bit),
				  false.DbType(SqlDbType.Bit),
				  data.Password.DbType(SqlDbType.NVarChar),
				  data2.Id.DbType(SqlDbType.Int));
		}

		[Table("Mailboxes", Schema = "mail")]
		public partial class Mailboxes
		{
			[Column("Id", TypeName = "int")]
			public int Id { get; set; }
			[Column("Username", TypeName = "nvarchar")]
			public string Username { get; set; }
			[Column("Password", TypeName = "nvarchar")]
			public string Password { get; set; }
			[Column("Name", TypeName = "nvarchar")]
			public string Name { get; set; }
			[Column("Quota", TypeName = "int")]
			public int Quota { get; set; }
			[Column("Domain", TypeName = "nvarchar")]
			public string Domain { get; set; }
			[Column("SubscriptionsId", TypeName = "int")]
			public int SubscriptionsId { get; set; }
			[Column("Created", TypeName = "datetime")]
			public DateTime Created { get; set; }
			[Column("Modified", TypeName = "datetime")]
			public DateTime? Modified { get; set; }
			[Column("LocalPart", TypeName = "nvarchar")]
			public string LocalPart { get; set; }
			[Column("UserDisabled", TypeName = "bit")]
			public bool UserDisabled { get; set; }
			[Column("Locked", TypeName = "bit")]
			public bool Locked { get; set; }
			[Column("Pending", TypeName = "bit")]
			public bool Pending { get; set; }
			[Column("UsedBytes", TypeName = "bigint")]
			public long UsedBytes { get; set; }
			[Column("LastUpdateUsedBytesTime", TypeName = "datetime2")]
			public DateTime LastUpdateUsedBytesTime { get; set; }
			[Column("ActualFeatureSetVersion", TypeName = "uniqueidentifier")]
			public Guid ActualFeatureSetVersion { get; set; }
			[Column("ManualLock", TypeName = "bit")]
			public bool ManualLock { get; set; }
			[Column("UserLocked", TypeName = "bit")]
			public bool UserLocked { get; set; }
			[Column("AdminLocked", TypeName = "bit")]
			public bool AdminLocked { get; set; }
			[Column("AutoLocked", TypeName = "bit")]
			public bool AutoLocked { get; set; }
		}

		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
		//	public void BuildSql_RenderNull()
		//	{
		//		Expression<Func<object>> exp = () => null;
		//		var builder = CreateBuilder();
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("NULL", result);
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		//	}
		//	[Fact]
		//	public void BuildSql_RenderIsNull()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.Picture == null;
		//		var builder = CreateBuilder();
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[Picture]ISNULL)", result);
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		//	}
		//	[Fact]
		//	public void BuildSql_RenderIsNotNull()
		//	{
		//		Expression<Func<Category, object>> exp = (c) => c.Picture != null;
		//		var builder = CreateBuilder();
		//		var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

		//		Assert.AreEqual("([c].[Picture]ISNOTNULL)", result);
		//		_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		//	}
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
		//	[Fact]
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
