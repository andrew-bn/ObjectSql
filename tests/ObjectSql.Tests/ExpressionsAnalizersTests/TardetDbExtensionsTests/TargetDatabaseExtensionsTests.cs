using NUnit.Framework;
using Moq;
using ObjectSql.Core;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core.SchemaManager;
using ObjectSql.SqlServer;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.QueryInterfaces;
using ObjectSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Tests.ExpressionsAnalizersTests.TardetDbExtensionsTests
{
	[TestFixture]
	public class TargetDatabaseExtensionsTests:TestBase
	{
		private Mock<IEntitySchemaManager> _schemaManager;
		private Mock<IDelegatesBuilder> _delegatesBuilder;
		private Mock<ICommandPreparatorsHolder> _parametersHolder;

		private List<CommandPrePostProcessor> _parameters;
		private Action<IDbCommand, object> _parameterFactory;
		private EntitySchema _categorySchema;
		private BuilderContext _builderContext;
		private int _parametersEncountered;
		[SetUp]
		public void Setup()
		{
			_categorySchema = new EntitySchema(typeof(Category), new StorageName(false,"Category", null),
									new Dictionary<string, StorageField>()
									{
										{ "CategoryID", new StorageField("CategoryID", null) }, 
										{ "CategoryName", new StorageField("CategoryName", null) }, 
										{ "Description", new StorageField("Description", null) },
										{ "Picture", new StorageField("Picture", null) },
									});
			_schemaManager = new Mock<IEntitySchemaManager>();
			_schemaManager.Setup(m => m.GetSchema(It.IsAny<Type>())).Returns(_categorySchema);

			_parameterFactory = (c, o) => { };
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

			_builderContext = new BuilderContext(new QueryContext(null, null, ResourcesTreatmentType.DisposeCommand,
				new QueryEnvironment(null, null, null, null)), null, null, null, null, null, null, null);
			_builderContext.Context.SqlPart = new SqlPart(_builderContext.Context);
			_builderContext.Preparators = _parametersHolder.Object;
			QueryRoots = _builderContext.Context.SqlPart.QueryRoots;
		}
		[Test]
		public void BuildSql_Avg()
		{
			Expression<Func<Category, object>> exp = (c) => Sql.Avg(c.CategoryID);
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("AVG([c].[CategoryID])", result);
		}
		[Test]
		public void BuildSql_Count()
		{
			Expression<Func<Category, object>> exp = c => Sql.Count(c.CategoryID);
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("COUNT([c].[CategoryID])", result);
		}
		[Test]
		public void BuildSql_Min()
		{
			Expression<Func<Category, object>> exp = ( c) => Sql.Min(c.CategoryID);
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("MIN([c].[CategoryID])", result);
		}
		[Test]
		public void BuildSql_Max()
		{
			Expression<Func<Category, object>> exp = (c) => Sql.Max(c.CategoryID);
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("MAX([c].[CategoryID])", result);
		}
		[Test]
		public void BuildSql_Like()
		{
			Expression<Func<Category, object>> exp = ( c) => c.CategoryName.Like("adf");
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryName]LIKE@p0)", result);
		}
		[Test]
		public void BuildSql_NotLike()
		{
			Expression<Func<Category, object>> exp = ( c) => c.CategoryName.NotLike("adf");
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("([c].[CategoryName]NOTLIKE@p0)", result);
		}
		protected Expression IsExp<T>(Expression<Func<T>> b)
		{
			return It.Is<Expression>(e => e.AreEqual(b));
		}

		private QueryExpressionBuilder CreateBuilder()
		{
			return new QueryExpressionBuilder(_schemaManager.Object, _delegatesBuilder.Object, SqlServerSqlWriter.Instance);
		}
	}
}
