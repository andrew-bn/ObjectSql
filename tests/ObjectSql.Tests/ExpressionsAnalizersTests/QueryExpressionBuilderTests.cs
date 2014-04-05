using NUnit.Framework;
using Moq;
using ObjectSql.Core;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Core.SchemaManager.EntitySchema;
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
	[TestFixture]
	public class QueryExpressionBuilderTests: TestBase
	{
		private Mock<IEntitySchemaManager> _schemaManager;
		private Mock<IDelegatesBuilder> _delegatesBuilder;
		private Mock<ICommandPreparatorsHolder> _parametersHolder;

		private List<CommandPrePostProcessor> _parameters;
		private Action<IDbCommand, object> _parameterFactory;
		private EntitySchema _categorySchema;
		private string _categoryNameField;
		private int _parametersEncountered;
		private BuilderContext _builderContext;
		
		[SetUp]
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
			_delegatesBuilder.Setup(b => b.CreateDatabaseParameterFactoryAction(It.IsAny<Expression>(), It.IsAny<Expression>(), It.IsAny<IStorageFieldType>(), ParameterDirection.Input))
				.Returns(_parameterFactory);

			_parameters = new List<CommandPrePostProcessor>();
			_parametersHolder = new Mock<ICommandPreparatorsHolder>();
			_parametersHolder.Setup(h => h.PreProcessors).Returns(_parameters);
			_parametersHolder.Setup(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()))
				.Callback((CommandPrePostProcessor d) => _parameters.Add(d));
			_parametersHolder.SetupSet(h => h.ParametersEncountered).Callback(i => _parametersEncountered = i);
			_parametersHolder.Setup(h => h.ParametersEncountered).Returns(() => _parametersEncountered);

			_builderContext = new BuilderContext(new QueryContext(null,null,ResourcesTreatmentType.DisposeCommand,
				new QueryEnvironment(null,null,null,null)), null, null, null, null, null, null, null);
			_builderContext.Context.SqlPart = new SqlPart(_builderContext.Context);
			_builderContext.Preparators = _parametersHolder.Object;
			QueryRoots = _builderContext.Context.SqlPart.QueryRoots;
		}
		[Test]
		public void BuildSql_RenderConstant()
		{
			Expression<Func<object>> exp = () => 5;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(5);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("@p0", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 5), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Once());
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderClassProperty()
		{
			var value = 5;
			Expression<Func<object>> exp = () => value;
			var builder = CreateBuilder();
			AddQueryRoot(() => value);

			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("@p0", result);
			_delegatesBuilder.Verify(b=>b.CreateDatabaseParameterFactoryAction(IsExp(()=>"p0"),IsExp(()=>value),null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Once());
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		
		[Test]
		public void BuildSql_RenderNestedClassProperty()
		{
			var value = new { Val = false };
			Expression<Func<object>> exp = () => value.Val;
			AddQueryRoot(()=>value);
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("@p0", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => value.Val), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Once());
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}

		[Test]
		public void BuildSql_RenderEntityField()
		{
			var value = new { Val = false };
			Expression<Func<Category,object>> exp = (catPar) => catPar.CategoryName;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true);

			Assert.AreEqual("[catPar].["+_categoryNameField+"]", result);
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		}

		[Test]
		public void BuildSql_RenderTwoParameters()
		{
			Expression<Func<Category,object>> exp = (c) => "val"+2;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("(@p0+@p1)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => "val"), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(2));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p1")));
		}
		[Test]
		public void BuildSql_RenderSameParameters()
		{
			Expression<Func<Category, object>> exp = (c) => "val" + 2 + "val";
			var builder = CreateBuilder();
			QueryRoots.AddRoot("val");
			QueryRoots.AddRoot(2);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => "val"), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(2));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p1")));
		}
		[Test]
		public void BuildSql_RenderSameParameterTypes()
		{
			Expression<Func<Category, object>> exp = (c) => "val" + 2 + "val2";
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p2)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => "val"), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p2"), IsExp(() => "val2"), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(3));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p1")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p2")));
		}
		[Test]
		public void BuildSql_RenderSameParameters_SameRoot_DifferentProperties()
		{
			var val1 = "val";
			var val2 = "val";
			Expression<Func<Category, object>> exp = (c) => val1 + 2 + val2;
			var builder = CreateBuilder();
			AddQueryRoot(()=>val1);
			QueryRoots.AddRoot(2);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p2)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => val1), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p2"), IsExp(() => val2), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(3));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p1")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p2")));
		}
		public class Root
		{
			public string Val { get; set; }
		}
		[Test]
		public void BuildSql_RenderSameRootTypes_SameParameterValues_DifferentRootInstances()
		{
			var v1 = new Root() { Val = "val" };
			var v2 = new Root() { Val = "val" };
			Expression<Func<Category, object>> exp = (c) => v1.Val + 2 + v2.Val;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p2)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => v1.Val), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p2"), IsExp(() => v2.Val), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(3));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p1")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p2")));
		}
		[Test]
		public void BuildSql_RenderSameRootParameters()
		{
			var v1 = new Root() { Val = "val" };
			Expression<Func<Category, object>> exp = (c) => v1.Val + 2 + v1.Val;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("((@p0+@p1)+@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => v1.Val), null, ParameterDirection.Input));
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p1"), IsExp(() => 2), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(2));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p1")));
		}
		[Test]
		public void BuildSql_RenderBinarySub()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID-1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]-@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderDivide()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID / 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]/@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderMult()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID * 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]*@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderGreater()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID > 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]>@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderGreaterOrEqual()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID >= 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]>=@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderLess()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID < 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext,exp.Parameters.ToArray(),  exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]<@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderLessOrEqual()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID <= 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]<=@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderNotEqual()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID != 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]<>@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		public enum Foo
		{
			Val1,
			Val2
		}
		[Test]
		public void BuildSql_RenderEqualToEnum()
		{
			Expression<Func<Category, object>> exp = (c) => c.CategoryID == (int)Foo.Val2;
			var builder = CreateBuilder();
			QueryRoots.AddRoot((int)Foo.Val2);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]=@p0)", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => (int)Foo.Val2), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderEqualToEnumVariable()
		{
			var enumVar = (Foo) Enum.Parse(typeof (Foo), "Val2");
			AddQueryRoot(()=>enumVar);
			Expression<Func<Category, object>> exp = (c) => c.CategoryID == (int)enumVar;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryID]=@p0)", result);
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderNull()
		{
			Expression<Func<object>> exp = () => null;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("NULL", result);
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		}
		[Test]
		public void BuildSql_RenderIsNull()
		{
			Expression<Func<Category, object>> exp = (c) => c.Picture == null;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[Picture]ISNULL)", result);
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		}
		[Test]
		public void BuildSql_RenderIsNotNull()
		{
			Expression<Func<Category, object>> exp = (c) => c.Picture != null;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[Picture]ISNOTNULL)", result);
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Never());
		}
		[Test]
		public void BuildSql_RenderAnd()
		{
			Expression<Func<Category, object>> exp = (c) => c.Description==null && c.CategoryID == 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("(([c].[Description]ISNULL)AND([c].[CategoryID]=@p0))", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderOr()
		{
			Expression<Func<Category, object>> exp = (c) => c.Description == null || c.CategoryID == 1;
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("(([c].[Description]ISNULL)OR([c].[CategoryID]=@p0))", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		[Test]
		public void BuildSql_RenderNot()
		{
			Expression<Func<Category, object>> exp = (c) => !(c.CategoryID == 1);
			var builder = CreateBuilder();
			QueryRoots.AddRoot(1);
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("(NOT([c].[CategoryID]=@p0))", result);
			_delegatesBuilder.Verify(b => b.CreateDatabaseParameterFactoryAction(IsExp(() => "p0"), IsExp(() => 1), null, ParameterDirection.Input));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()), Times.Exactly(1));
			_parametersHolder.Verify(h => h.AddPreProcessor(It.Is<CommandPrePostProcessor>(d => ((CommandParameterPreProcessor)d).Name == "p0")));
		}
		public int GetConstant()
		{
			return 0;
		}
		[Test]
		public void BuildSql_MethodCall_ErrorExpected()
		{
			Expression<Func<object>> exp = () => this.GetConstant();
			var builder = CreateBuilder();
			builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();
		}
		
		protected Expression IsExp<T>(Expression<Func<T>> b)
		{
			return It.Is<Expression>(e=>e.AreEqual(b));
		}

		private QueryExpressionBuilder CreateBuilder()
		{
			return new QueryExpressionBuilder(_schemaManager.Object, _delegatesBuilder.Object, SqlServerSqlWriter.Instance);
		}
	}
	[DatabaseExtension]
	public class TestExtension
	{
		public static bool LikeForTests(string left, string right)
		{
			return false;
		}
		public static bool RenderLikeWasCalled = false;
		internal static string RenderLikeForTests(BuilderContext commandPreparators, string[] parts)
		{
			RenderLikeWasCalled = true;
			return string.Format("({0} LIKE {1})",parts[0], parts[1]);
		}
	}
}
