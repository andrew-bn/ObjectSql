using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlBoost.Core.Bo.CommandPreparatorDescriptor;
using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core;
using SqlBoost.Core.QueryBuilder.LambdaBuilder;
using SqlBoost.Core.SchemaManager;
using SqlBoost.QueryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using SqlBoost.Core.QueryBuilder.ExpressionsAnalizers;
using SqlBoost.SqlServer;

namespace SqlBoost.Tests.SqlServerTests
{
	[TestClass]
	public class SqlServerTargetDatabaseExtensionsTests
	{
		private Mock<IEntitySchemaManager> _schemaManager;
		private Mock<IDelegatesBuilder> _delegatesBuilder;
		private Mock<ICommandPreparatorsHolder> _parametersHolder;

		private List<CommandPreparator> _parameters;
		private Action<IDbCommand, object> _parameterFactory;
		private EntitySchema _categorySchema;
		private int _parametersEncountered;
		[TestInitialize]
		public void Setup()
		{
			_categorySchema = new EntitySchema(typeof(Category), new StorageName("Category", null),
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
		public void BuildSql_CountBig()
		{
			Expression<Func<Category, object>> exp = (c) => MsSql.CountBig(c.CategoryID);
			var result = CreateBuilder().BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("COUNT_BIG([c].[CategoryID])", result);
		}
		[TestMethod]
		public void BuildSql_Lower()
		{
			Expression<Func<Category, object>> exp = (c) => MsSql.Lower(c.Description);
			var result = CreateBuilder().BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("LOWER([c].[Description])", result);
		}
		[TestMethod]
		public void BuildSql_Replace()
		{
			Expression<Func<Category, object>> exp = c => MsSql.Replace(c.Description,"p","c");
			var result = CreateBuilder().BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("REPLACE([c].[Description],@p0,@p1)", result);
		}
		[TestMethod]
		public void BuildSql_Substring()
		{
			Expression<Func<Category, object>> exp = ( c) => MsSql.Substring(c.Description,1,2);
			var result = CreateBuilder().BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("SUBSTRING([c].[Description],@p0,@p1)", result);
		}
		[TestMethod]
		public void BuildSql_Upper()
		{
			Expression<Func<Category, object>> exp = ( c) => MsSql.Upper(c.CategoryName);
			var result = CreateBuilder().BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("UPPER([c].[CategoryName])", result);
		}

		protected Expression IsExp<T>(Expression<Func<T>> b)
		{
			return It.Is<Expression>(e => e.AreEqual(b));
		}

		private QueryExpressionBuilder CreateBuilder()
		{
			return new QueryExpressionBuilder(_schemaManager.Object, _delegatesBuilder.Object, new SqlServerSqlWriter());
		}
	}
}
