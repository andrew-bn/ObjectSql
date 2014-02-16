using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ObjectSql.Core;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Exceptions;
using ObjectSql.QueryInterfaces;
using ObjectSql.SqlServer;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Tests.ExpressionsAnalizersTests
{
	[TestClass]
	public class QueryExpressionBuilderTests: TestBase
	{
		private Mock<IEntitySchemaManager> _schemaManager;
		private Mock<IDelegatesBuilder> _delegatesBuilder;
		private Mock<ICommandPreparatorsHolder> _parametersHolder;

		private List<CommandPreparator> _parameters;
		private Action<IDbCommand, object> _parameterFactory;
		private EntitySchema _categorySchema;
		private string _categoryNameField;
		private int _parametersEncountered;
		[TestInitialize]
		public void Setup()
		{
			_categoryNameField = "Category Name Fld";
			_categorySchema = new EntitySchema(typeof(Category), new StorageName(false,"Category", null),
									new Dictionary<string, StorageField>()
									{
										{ "CategoryID", new StorageField("CategoryID", null) }, 
										{ "CategoryName", new StorageField(_categoryNameField, null) }, 
										{ "Description", new StorageField("Description", null) },
										{ "Picture", new StorageField("Picture", null) },
									});
			_schemaManager = new Mock<IEntitySchemaManager>();
			_schemaManager.Setup(m => m.GetSchema(It.IsAny<Type>())).Returns(_categorySchema);

			_parameterFactory = (c,o) => {};
			_delegatesBuilder = new Mock<IDelegatesBuilder>();
			_delegatesBuilder.Setup(b => b.CreateDatabaseParameterFactoryAction(It.IsAny<Expression>(), It.IsAny<Expression>(), It.IsAny<IStorageFieldType>()))
				.Returns(_parameterFactory);

			_parameters = new List<CommandPreparator>();
			_parametersHolder = new Mock<ICommandPreparatorsHolder>();
			_parametersHolder.Setup(h => h.Preparators).Returns(_parameters);
			_parametersHolder.Setup(h => h.AddPreparator(It.IsAny<CommandPreparator>()))
				.Callback((CommandPreparator d) => _parameters.Add(d));
			_parametersHolder.SetupSet(h => h.ParametersEncountered).Callback(i => _parametersEncountered = i);
			_parametersHolder.Setup(h => h.ParametersEncountered).Returns(() => _parametersEncountered);

		}
		[TestMethod]
		public void BuildSql_RenderConstant()
		{
			Expression<Func<object>> exp = () => 5;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("@p0", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 5), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Once());
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderClassProperty()
		{
			var value = 5;
			Expression<Func<object>> exp = () => value;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("@p0", result);
			_delegatesBuilder.Verify(b=>b.CreateDatabaseParameterFactoryAction(IsExp(()=>"p0"),IsExp(()=>value),null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Once());
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderNestedClassProperty()
		{
			var value = new { Val = false };
			Expression<Func<object>> exp = () => value.Val;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("@p0", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => value.Val), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Once());
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}

		[TestMethod]
		public void BuildSql_RenderEntityField()
		{
			var value = new { Val = false };
			Expression<Func<Category,object>> exp = (catPar) => catPar.CategoryName;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true);

			Assert.AreEqual("[catPar].["+_categoryNameField+"]", result);
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Never());
		}

		[TestMethod]
		public void BuildSql_RenderTwoParameters()
		{
			Expression<Func<Category,object>> exp = (c) => "val"+2;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("(@p0+@p1)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => "val"), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(2));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p1")));
		}
		[TestMethod]
		public void BuildSql_RenderSameParameters()
		{
			Expression<Func<Category, object>> exp = (c) => "val" + 2 + "val";
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => "val"), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(2));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p1")));
		}
		[TestMethod]
		public void BuildSql_RenderSameParameterTypes()
		{
			Expression<Func<Category, object>> exp = (c) => "val" + 2 + "val2";
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p2)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => "val"), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p2"), IsExp(() => "val2"), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(3));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p1")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p2")));
		}
		[TestMethod]
		public void BuildSql_RenderSameParameters_SameRoot_DifferentProperties()
		{
			var val1 = "val";
			var val2 = "val";
			Expression<Func<Category, object>> exp = (c) => val1 + 2 + val2;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p2)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => val1), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p2"), IsExp(() => val2), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(3));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p1")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p2")));
		}
		public class Root
		{
			public string Val { get; set; }
		}
		[TestMethod]
		public void BuildSql_RenderSameRootTypes_SameParameterValues_DifferentRootInstances()
		{
			var v1 = new Root() { Val = "val" };
			var v2 = new Root() { Val = "val" };
			Expression<Func<Category, object>> exp = (c) => v1.Val + 2 + v2.Val;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p2)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => v1.Val), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p2"), IsExp(() => v2.Val), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(3));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p1")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p2")));
		}
		[TestMethod]
		public void BuildSql_RenderSameRootParameters()
		{
			var v1 = new Root() { Val = "val" };
			Expression<Func<Category, object>> exp = (c) => v1.Val + 2 + v1.Val;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => v1.Val), null));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(2));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p1")));
		}
		[TestMethod]
		public void BuildSql_RenderBinarySub()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID-1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]-@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderDivide()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID / 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]/@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderMult()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID * 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]*@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderGreater()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID > 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]>@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderGreaterOrEqual()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID >= 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]>=@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderLess()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID < 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]<@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderLessOrEqual()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID <= 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]<=@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderNotEqual()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID != 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]<>@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderNull()
		{
			Expression<Func<object>> exp = () => null;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("NULL", result);
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Never());
		}
		[TestMethod]
		public void BuildSql_RenderIsNull()
		{
			Expression<Func<Category, object>> exp = (c) => c.Picture == null;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[Picture]ISNULL)", result);
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Never());
		}
		[TestMethod]
		public void BuildSql_RenderIsNotNull()
		{
			Expression<Func<Category, object>> exp = (c) => c.Picture != null;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[Picture]ISNOTNULL)", result);
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Never());
		}
		[TestMethod]
		public void BuildSql_RenderAnd()
		{
			Expression<Func<Category, object>> exp = (c) => c.Description==null && c.CategoryID == 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("(([c].[Description]ISNULL)AND([c].[CategoryID]=@p0))", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderOr()
		{
			Expression<Func<Category, object>> exp = (c) => c.Description == null || c.CategoryID == 1;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("(([c].[Description]ISNULL)OR([c].[CategoryID]=@p0))", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		[TestMethod]
		public void BuildSql_RenderNot()
		{
			Expression<Func<Category, object>> exp = (c) => !(c.CategoryID == 1);
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("(NOT([c].[CategoryID]=@p0))", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		public int GetConstant()
		{
			return 0;
		}
		[TestMethod]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_MethodCall_ErrorExpected()
		{
			Expression<Func<object>> exp = () => this.GetConstant();
			var builder = CreateBuilder();
			builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();
		}
		
		[TestMethod]
		public void BuildSql_TargetDatabaseExtensionMethodCall_ValidResult()
		{
			TestExtension.RenderLikeWasCalled = false;
			Expression<Func<Category,object>> exp = (c) => TestExtension.LikeForTests(c.Description,"%value%");
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("([c].[Description]LIKE@p0)", result);
			Assert.IsTrue(TestExtension.RenderLikeWasCalled);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => "%value%"), null));
			_parametersHolder.Verify(h => h.AddPreparator(It.IsAny<CommandPreparator>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreparator(It.Is<CommandPreparator>(d => ((SingleParameterPreparator)d).Name == "p0")));
		}
		protected Expression IsExp<T>(Expression<Func<T>> b)
		{
			return It.Is<Expression>(e=>e.AreEqual(b));
		}

		private QueryExpressionBuilder CreateBuilder()
		{
			return new QueryExpressionBuilder(_schemaManager.Object, _delegatesBuilder.Object, new SqlServerSqlWriter());
		}
	}
	public class TestExtension:DatabaseExtension
	{
		public static bool LikeForTests(string left, string right)
		{
			return false;
		}
		public static bool RenderLikeWasCalled = false;
		internal static string RenderLikeForTests(ICommandPreparatorsHolder commandPreparators, string[] parts)
		{
			RenderLikeWasCalled = true;
			return string.Format("({0} LIKE {1})",parts[0], parts[1]);
		}
	}
}
