using Microsoft.VisualStudio.TestTools.UnitTesting;
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
	[TestClass]
	public class QueryUpdateBuilderTests:TestBase 
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
		[TestMethod]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_NoMemberInitNode_ErrorExpected()
		{
			var c = new Category();
			Expression<Func<Category>> exp = () => c;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true);
		}
		[TestMethod]
		public void BuildSql_ValidResultExpected()
		{
			var c = new Category();
			Expression<Func<Category>> exp = () => new Category { CategoryID = 2, CategoryName = "cn"};
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("[CategoryID]=@p0,[CategoryNameFld]=@p1", result);
		}
		private QueryUpdateBuilder CreateBuilder()
		{
			return new QueryUpdateBuilder(_schemaManager.Object, _delegatesBuilder.Object,new SqlServerSqlWriter());
		}
		protected Expression IsExp<T>(Expression<Func<T>> b)
		{
			return It.Is<Expression>(e => e.AreEqual(b));
		}
	}
}
