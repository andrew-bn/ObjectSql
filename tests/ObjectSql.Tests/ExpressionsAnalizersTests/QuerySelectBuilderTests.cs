﻿using NUnit.Framework;
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
	public class QuerySelectBuilderTests
	{
		private Mock<IEntitySchemaManager> _schemaManager;
		private Mock<IDelegatesBuilder> _delegatesBuilder;
		private Mock<ICommandPreparatorsHolder> _parametersHolder;

		private List<CommandPreparator> _parameters;
		private Action<IDbCommand, object> _parameterFactory;
		private EntitySchema _categorySchema;
		private string _categoryNameField;
		private int _parametersEncountered;
		[SetUp]
		public void Setup()
		{
			_categoryNameField = "Category Name Fld";
			_categorySchema = new EntitySchema(typeof(Category), new StorageName(false, "Category", null),
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
		[Test]
		public void BuildSql_SelectNew_ConstructorInitializer()
		{
			Expression<Func<Dto>> exp = () => new Dto(3, "name");
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("@p0AS[identity],@p1AS[dtoName]", result);
		}
		[Test]
		public void BuildSql_SelectNew_ParametersInitializer()
		{
			Expression<Func<Dto>> exp = () => new Dto { Id = 2, Name = "name" };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("@p0AS[Id],@p1AS[Name]", result);
		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_SelectNew_ParametersAndConstructorInitializer_ErrorExpected()
		{
			Expression<Func<Dto>> exp = () => new Dto(4, "name") { Name = "name" };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();
		}
		[Test]
		public void BuildSql_SelectAnonimus_ParametersInitializer()
		{
			Expression<Func<object>> exp = () => new { Id = 2, Name = "name" };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("@p0AS[Id],@p1AS[Name]", result);
		}
		[Test]
		public void BuildSql_SelectParameter()
		{
			Expression<Func<Category, Category>> exp = (p) => p;
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();

			Assert.AreEqual("[p].[CategoryID],[p].[CategoryNameFld],[p].[Description],[p].[Picture]", result);
		}

		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void BuildSql_SelectNestedAnonimus_ErrorExpected()
		{
			Expression<Func<object>> exp = () => new { Id = 2, Name = "name", D = new { Descr = "descr" } };
			var builder = CreateBuilder();
			var result = builder.BuildSql(_parametersHolder.Object, exp.Body, true).Prepare();
		}

		private QuerySelectBuilder CreateBuilder()
		{
			return new QuerySelectBuilder(_schemaManager.Object, _delegatesBuilder.Object, SqlServerSqlWriter.Instance);

		}
		protected Expression IsExp<T>(Expression<Func<T>> b)
		{
			return It.Is<Expression>(e => e.AreEqual(b));
		}
	}
}