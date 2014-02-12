using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlBoost.Core;
using SqlBoost.Core.Bo;
using SqlBoost.Core.Bo.CommandPreparatorDescriptor;
using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core.QueryBuilder;
using SqlBoost.Core.QueryBuilder.ExpressionsAnalizers;
using SqlBoost.Core.QueryBuilder.LambdaBuilder;
using SqlBoost.Core.SchemaManager;
using SqlBoost.SqlServer;
using SqlBoost.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost.Tests.ExpressionsAnalizersTests
{
	[TestClass]
	public class QueryFieldsSequenceBuilderTests
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
			_categorySchema = new EntitySchema(typeof(Category), new StorageName("Category", null),
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
		public class Dto
		{
			public Dto() { }
			public Dto(int identity, string dtoName) { }
			public int Id { get; set; }
			public string Name { get; set; }
		}
		[TestMethod]
		[ExpectedException(typeof(SqlBoostException))]
		public void BuildSql_SelectNewDto_MemberInit_ErrorExpected()
		{
			Expression<Func<Dto, object>> exp = (d) => new Dto {Id = 2 };
			var builder = CreateBuilder();
			builder.BuildSql(_parametersHolder.Object, exp.Body, true);
		}
		[TestMethod]
		[ExpectedException(typeof(SqlBoostException))]
		public void BuildSql_SelectNewDto_ConstructorInitializer_ErrorExpected()
		{
			Expression<Func<Dto, object>> exp = (d) => new Dto(1,"");
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true);
		}
		[TestMethod]
		public void BuildSql_SelectNewAnonimus_ValidResult()
		{
			Expression<Func<Category, object>> exp = (d) => new { d.Description, d.Picture };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, false).Prepare();

			Assert.AreEqual("[Description],[Picture]", result);
		}
		[TestMethod]
		[ExpectedException(typeof(SqlBoostException))]
		public void BuildSql_SelectNewAnonimus_ConstantFieldAsSelectedField_ErrorExpected()
		{
			var c = 4;
			Expression<Func<Category, object>> exp = (d) => new { c, d.Picture };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, false);
		}
		[TestMethod]
		[ExpectedException(typeof(SqlBoostException))]
		public void BuildSql_SelectNewAnonimus_ConstantAsSelectedField_ErrorExpected()
		{
			Expression<Func<Category, object>> exp = (d) => new { fld = 5, d.Picture };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, false);
		}
		[TestMethod]
		[ExpectedException(typeof(SqlBoostException))]
		public void BuildSql_SelectNewComplexAnonimus_ErrorExpected()
		{
			Expression<Func<Category, object>> exp = (d) => new { d.Picture, fld = new { d.Description } };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, false);
		}
		private QueryFieldsSequenceBuilder CreateBuilder()
		{
			return new QueryFieldsSequenceBuilder(_schemaManager.Object, _delegatesBuilder.Object, new SqlServerSqlWriter());

		}
		protected Expression IsExp<T>(Expression<Func<T>> b)
		{
			return It.Is<Expression>(e => e.AreEqual(b));
		}
	}
}
