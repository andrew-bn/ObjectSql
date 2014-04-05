﻿using System.Linq;
using NUnit.Framework;
using Moq;
using ObjectSql.Core;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core.SchemaManager;
using ObjectSql.SqlServer;
using ObjectSql.QueryInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace ObjectSql.Tests.SqlServerTests
{
	[TestFixture]
	public class SqlServerTargetDatabaseExtensionsTests: TestBase
	{
		private Mock<IEntitySchemaManager> _schemaManager;
		private Mock<IDelegatesBuilder> _delegatesBuilder;
		private Mock<ICommandPreparatorsHolder> _parametersHolder;

		private List<CommandPrePostProcessor> _parameters;
		private Action<IDbCommand, object> _parameterFactory;
		private EntitySchema _categorySchema;
		private EntitySchema _employeeSchema;
		private int _parametersEncountered;
		private BuilderContext _builderContext;
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
			_employeeSchema = new EntitySchema(typeof(Employee), new StorageName(false, "Employee", null),
									new Dictionary<string, StorageField>()
									{
										{ "BirthDate", new StorageField("BirthDate", null) }
									});
			_schemaManager = new Mock<IEntitySchemaManager>();
			_schemaManager.Setup(m => m.GetSchema(typeof(Category))).Returns(_categorySchema);
			_schemaManager.Setup(m => m.GetSchema(typeof(Employee))).Returns(_employeeSchema);

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
		public void BuildSql_CountBig()
		{
			Expression<Func<Category, object>> exp = (c) => MsSql.CountBig(c.CategoryID);
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("COUNT_BIG([c].[CategoryID])", result);
		}
		[Test]
		public void BuildSql_Lower()
		{
			Expression<Func<Category, object>> exp = (c) => MsSql.Lower(c.Description);
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("LOWER([c].[Description])", result);
		}
		[Test]
		public void BuildSql_Replace()
		{
			Expression<Func<Category, object>> exp = c => MsSql.Replace(c.Description,"p","c");
			QueryRoots.AddRoot("p");
			QueryRoots.AddRoot("c");
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("REPLACE([c].[Description],@p0,@p1)", result);
		}
		[Test]
		public void BuildSql_Substring()
		{
			Expression<Func<Category, object>> exp = ( c) => MsSql.Substring(c.Description,1,2);
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("SUBSTRING([c].[Description],@p0,@p1)", result);
		}
		[Test]
		public void BuildSql_Upper()
		{
			Expression<Func<Category, object>> exp = ( c) => MsSql.Upper(c.CategoryName);
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("UPPER([c].[CategoryName])", result);
		}
		[Test]
		public void BuildSql_DateDiff_GetDate()
		{
			Expression<Func<Employee, object>> exp = c => MsSql.DateDiff(DatePart.Day, c.BirthDate, MsSql.GetDate());
			var result = CreateBuilder().BuildSql(_builderContext, exp.Parameters.ToArray(), exp.Body, true).Prepare();

			Assert.AreEqual("DATEDIFF(day,[c].[BirthDate],GETDATE())", result);
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
