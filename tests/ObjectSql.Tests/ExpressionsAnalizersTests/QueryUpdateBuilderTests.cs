//using NUnit.Framework;
//using Moq;
//using ObjectSql.Core;
//using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
//using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
//using ObjectSql.Core.QueryBuilder.LambdaBuilder;
//using ObjectSql.Core.QueryParts;
//using ObjectSql.Core.SchemaManager;
//using ObjectSql.Core.SchemaManager.EntitySchema;
//using ObjectSql.Exceptions;
//using ObjectSql.SqlServer;
//using ObjectSql.Core.Bo;
//using ObjectSql.Core.QueryBuilder;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace ObjectSql.Tests.ExpressionsAnalizersTests
//{
//	[TestFixture]
//	public class QueryUpdateBuilderTests:TestBase 
//	{
//		private Mock<IEntitySchemaManager> _schemaManager;
//		private Mock<IDelegatesBuilder> _delegatesBuilder;
//		private Mock<ICommandPreparatorsHolder> _parametersHolder;

//		private List<CommandPrePostProcessor> _parameters;
//		private Action<IDbCommand, object> _parameterFactory;
//		private EntitySchema _categorySchema;
//		private string _categoryNameField;
//		private int _parametersEncountered;
//		private BuilderContext _builderContext;
//		[SetUp]
//		public void Setup()
//		{
//			_categoryNameField = "Category Name Fld";
//			_categorySchema = new EntitySchema(typeof(Category), new StorageName(false,"Category", null),
//									new Dictionary<string, StorageField>()
//									{
//										{ "CategoryID", new StorageField("CategoryID", null) }, 
//										{ "CategoryName", new StorageField(_categoryNameField, null) }, 
//										{ "Description", new StorageField("Description", null) },
//										{ "Picture", new StorageField("Picture", null) },
//									});
//			_schemaManager = new Mock<IEntitySchemaManager>();
//			_schemaManager.Setup(m => m.GetSchema(It.IsAny<Type>())).Returns(_categorySchema);

//			_parameterFactory = (c, o) => { };
//			_delegatesBuilder = new Mock<IDelegatesBuilder>();
//			_delegatesBuilder.Setup(b => b.CreateDatabaseParameterFactoryAction(It.IsAny<Expression>(), It.IsAny<Expression>(), It.IsAny<IStorageFieldType>(), ParameterDirection.Input))
//				.Returns(_parameterFactory);

//			_parameters = new List<CommandPrePostProcessor>();
//			_parametersHolder = new Mock<ICommandPreparatorsHolder>();
//			_parametersHolder.Setup(h => h.PreProcessors).Returns(_parameters);
//			_parametersHolder.Setup(h => h.AddPreProcessor(It.IsAny<CommandPrePostProcessor>()))
//				.Callback((CommandPrePostProcessor d) => _parameters.Add(d));
//			_parametersHolder.SetupSet(h => h.ParametersEncountered).Callback(i => _parametersEncountered = i);
//			_parametersHolder.Setup(h => h.ParametersEncountered).Returns(() => _parametersEncountered);

//			_builderContext = new BuilderContext(new QueryContext(null, null, ResourcesTreatmentType.DisposeCommand,
//				new QueryEnvironment(null, null, null, null)), null, null, null, null, null, null, null);
//			_builderContext.Context.SqlPart = new SqlPart(_builderContext.Context);
//			_builderContext.Preparators = _parametersHolder.Object;
//			QueryRoots = _builderContext.Context.SqlPart.QueryRoots;
//		}
//		[Test]
//		[ExpectedException(typeof(ObjectSqlException))]
//		public void BuildSql_NoMemberInitNode_ErrorExpected()
//		{
//			var c = new Category();
//			Expression<Func<Category>> exp = () => c;
//			AddQueryRoot(()=>c);
//			var builder = CreateBuilder();

//			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body);
//		}
//		[Test]
//		public void BuildSql_ValidResultExpected()
//		{
//			var c = new Category();
//			Expression<Func<Category>> exp = () => new Category { CategoryID = 2, CategoryName = "cn"};
//			QueryRoots.AddRoot(2);
//			QueryRoots.AddRoot("cn");
//			var builder = CreateBuilder();
//			var result = builder.BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body).Prepare();

//			Assert.AreEqual("[CategoryID]=@p0,[CategoryNameFld]=@p1", result);
//		}
//		private QueryUpdateBuilder CreateBuilder()
//		{
//			return new QueryUpdateBuilder(_schemaManager.Object, _delegatesBuilder.Object, SqlServerSqlWriter.Instance);
//		}
//		protected Expression IsExp<T>(Expression<Func<T>> b)
//		{
//			return It.Is<Expression>(e => e.AreEqual(b));
//		}
//	}
//}
