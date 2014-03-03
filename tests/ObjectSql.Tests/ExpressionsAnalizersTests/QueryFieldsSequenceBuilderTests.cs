using NUnit.Framework;
using Moq;
using ObjectSql.Core;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Exceptions;
using ObjectSql.SqlServer;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Tests.ExpressionsAnalizersTests
{
	[TestFixture]
	public class QueryFieldsSequenceBuilderTests
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

			_builderContext = new BuilderContext(null, null, null, null, null, null, null, null);
			_builderContext.Preparators = _parametersHolder.Object;
		}
		public class Dto
		{
			public Dto() { }
			public Dto(int identity, string dtoName) { }
			public int Id { get; set; }
			public string Name { get; set; }
		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_SelectNewDto_MemberInit_ErrorExpected()
		{
			Expression<Func<Dto, object>> exp = (d) => new Dto {Id = 2 };
			var builder = CreateBuilder();
			builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true);
		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_SelectNewDto_ConstructorInitializer_ErrorExpected()
		{
			Expression<Func<Dto, object>> exp = (d) => new Dto(1,"");
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true);
		}
		[Test]
		public void BuildSql_SelectNewAnonimus_ValidResult()
		{
			Expression<Func<Category, object>> exp = (d) => new { d.Description, d.Picture };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, false).Prepare();

			Assert.AreEqual("[Description],[Picture]", result);
		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_SelectNewAnonimus_ConstantFieldAsSelectedField_ErrorExpected()
		{
			var c = 4;
			Expression<Func<Category, object>> exp = (d) => new { c, d.Picture };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, false);
		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_SelectNewAnonimus_ConstantAsSelectedField_ErrorExpected()
		{
			Expression<Func<Category, object>> exp = (d) => new { fld = 5, d.Picture };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, false);
		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_SelectNewComplexAnonimus_ErrorExpected()
		{
			Expression<Func<Category, object>> exp = (d) => new { d.Picture, fld = new { d.Description } };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, false);
		}
		private QueryFieldsSequenceBuilder CreateBuilder()
		{
			return new QueryFieldsSequenceBuilder(_schemaManager.Object, _delegatesBuilder.Object, SqlServerSqlWriter.Instance);

		}
		protected Expression IsExp<T>(Expression<Func<T>> b)
		{
			return It.Is<Expression>(e => e.AreEqual(b));
		}
	}
}
