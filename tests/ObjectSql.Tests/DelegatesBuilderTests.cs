using NUnit.Framework;
using Moq;
using ObjectSql.Core;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Exceptions;
using ObjectSql.Core.SchemaManager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Tests
{
	[TestFixture]
	public class DelegatesBuilderTests
	{
		public class Root
		{
			public string Value {get;set;}
		}
		public class FakeParameter
		{
			public string Name { get; private set; }
			public object Value { get; private set; }
			internal IStorageFieldType Type { get; private set; }
			internal FakeParameter(string name, IStorageFieldType type, object value)
			{
				Name = name;
				Type = type;
				Value = value;
			}
		}
		public class Entity
		{
			public Entity()
			{
			}
			public Entity(int id,string descr)
			{
				Id = id;
				Descr = descr;
			}
			public int Id {get;set;}
			public string Name{get;set;}
			public string Descr { get; set; }
			public bool Ignore {get;set;}
		}
		private Mock<IStorageFieldType> _fieldType;
		private Mock<IDbCommand> _command;
		private Mock<IDataParameterCollection> _parameterCollection;
		private Mock<IDataReader> _dataReader;

		private Expression _paramName;
		private Expression _valueAccessor;

		private Root _root;
		private string _parameterName;

		private EntitySchema _entitySchema;
		private Expression ParameterFactory(Expression name, Expression value, IStorageFieldType type)
		{
			return LambdaExpression.New(Reflect.FindCtor(() => new FakeParameter("", null, null)), name,
							 LambdaExpression.Constant(type,typeof(IStorageFieldType)), Expression.Convert(value,typeof(object)));
		}
		[SetUp]
		public void Setup()
		{
			_fieldType = new Mock<IStorageFieldType>();
			_entitySchema = new EntitySchema(typeof(Entity),new StorageName(false,"Entity",null),
				new Dictionary<string, StorageField>() 
				{ 
					{ "Id", new StorageField("Id", _fieldType.Object) }, 
					{ "Name", new StorageField("Name", _fieldType.Object) }, 
					{ "Descr", new StorageField("Descr", null) },
					{ "Ignore", new StorageField("Ignore", null) },
				});
			_parameterName = "Param name";
			_root = new Root() { Value = "value" };


			_paramName = LambdaExpression.Constant(_parameterName);
			_valueAccessor = LambdaExpression.MakeMemberAccess(LambdaExpression.Constant(_root), Reflect.FindProperty(() => _root.Value));

			
			_parameterCollection = new Mock<IDataParameterCollection>();
			_command = new Mock<IDbCommand>();
			_command.Setup(c => c.Parameters).Returns(_parameterCollection.Object);
			_dataReader = new Mock<IDataReader>();
		}
		#region CreateDatabaseParameterFactoryAction
		[Test]
		public void CreateParameterToCommandAppender_CreatesParameterFactory_ValidCall()
		{
			var builder = CreateBuilder();
			bool validCall = false;
			builder.CreateParameterFactoryFunc = 
				(pName, pAccessor, dbType) => 
				{
					validCall = pName == _paramName && dbType == _fieldType.Object &&
								pAccessor.NodeType == ExpressionType.MemberAccess &&
								pAccessor.Type == typeof(string);
					var nextExp = ((MemberExpression)pAccessor).Expression;
					validCall = validCall && nextExp.NodeType == ExpressionType.Convert &&
								nextExp.Type == typeof(Root);
					nextExp = ((UnaryExpression)nextExp).Operand;
					validCall = validCall && nextExp.NodeType == ExpressionType.Parameter &&
									nextExp.Type == typeof(object);
					return ParameterFactory(pName,pAccessor,dbType);
				};

			builder.CreateDatabaseParameterFactoryAction(_paramName, _valueAccessor, _fieldType.Object, ParameterDirection.Input);

			Assert.IsTrue(validCall);
		}
		[Test]
		public void CreateParameterToCommandAppender_ValidDelegateCreated()
		{
			var builder = CreateBuilder();
			builder.CreateParameterFactoryFunc = (pName, pAccessor, dbType) => ParameterFactory(pName, pAccessor, dbType);

			var result = builder.CreateDatabaseParameterFactoryAction(_paramName, _valueAccessor, _fieldType.Object, ParameterDirection.Input);
			
			result(_command.Object, _root);

			_parameterCollection.Verify(c => c.Add(It.Is<FakeParameter>(p=>p.Name ==_parameterName && p.Value == _root.Value && p.Type == _fieldType.Object )));
		}
		public int GetConstant()
		{
			return 234;
		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void CreateParameterToCommandAppender_MethodCallConstant_ErrorExpected()
		{
			Expression<Func<int>> exp = () => this.GetConstant();
			_valueAccessor = exp.Body;

			var builder = CreateBuilder();
			builder.CreateParameterFactoryFunc = (pName, pAccessor, dbType) => ParameterFactory(pName, pAccessor, dbType);

			var result = builder.CreateDatabaseParameterFactoryAction(_paramName, _valueAccessor, _fieldType.Object, ParameterDirection.Input);

		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void CreateParameterToCommandAppender_ValueAccessorRootIsParameter_ErrorExpected()
		{
			Expression<Func<Root, string>> exp = (p) => p.Value;
			_valueAccessor = exp.Body;

			var builder = CreateBuilder();
			builder.CreateParameterFactoryFunc = (pName, pAccessor, dbType) => ParameterFactory(pName, pAccessor, dbType);

			var result = builder.CreateDatabaseParameterFactoryAction(_paramName, _valueAccessor, _fieldType.Object, ParameterDirection.Input);
		}
		public class ValueHolder
		{
			public static string Value { get; set; }
		}
		[Test]
		[ExpectedException(typeof(ObjectSqlException))]
		public void CreateParameterToCommandAppender_ValueAccessorRootIsStatic_ErrorExpected()
		{
			Expression<Func<string>> exp = () => ValueHolder.Value;
			_valueAccessor = exp.Body;

			var builder = CreateBuilder();
			builder.CreateParameterFactoryFunc = (pName, pAccessor, dbType) => ParameterFactory(pName, pAccessor, dbType);

			var result = builder.CreateDatabaseParameterFactoryAction(_paramName, _valueAccessor, _fieldType.Object, ParameterDirection.Input);
		}
		#endregion
		#region GenerateMaterializationDelegate
		[Test]
		public void GenerateMaterializationDelegate_CtorBasedMaterialization_ValidDelegateGenerated()
		{
			
			var info = new EntityMaterializationInformation(Reflect.FindCtor(()=>new Entity(1,"")));
			var builder = CreateBuilder();
			var result = builder.CreateEntityMaterializationDelegate(_entitySchema, info);
			((Func<IDataReader, Entity>)result)(_dataReader.Object);

			_dataReader.Verify(r => r.GetInt32(0));
			_dataReader.Verify(r => r.GetString(1));
		}
		[Test]
		public void GenerateMaterializationDelegate_CtorBasedMaterialization_ValidEntityCreated()
		{
			var expectedId = 23412;
			var expectedDescr = "description";
		
			_dataReader.Setup(r => r.GetInt32(It.IsAny<int>())).Returns(expectedId);
			_dataReader.Setup(r => r.GetString(It.IsAny<int>())).Returns(expectedDescr);

			var info = new EntityMaterializationInformation(Reflect.FindCtor(() => new Entity(1, "")));
			var builder = CreateBuilder();
			var result = builder.CreateEntityMaterializationDelegate(_entitySchema, info);
			var entity = ((Func<IDataReader, Entity>)result)(_dataReader.Object);

			Assert.AreEqual(expectedId, entity.Id);
			Assert.AreEqual(expectedDescr, entity.Descr);
			Assert.AreEqual(null, entity.Name);
			Assert.AreEqual(false,entity.Ignore);
		}

		[Test]
		public void GenerateMaterializationDelegate_ParamsBasedMaterialization_ValidDelegateGenerated()
		{

			var info = new EntityMaterializationInformation(new[]{0,1,3});
			var builder = CreateBuilder();
			var result = builder.CreateEntityMaterializationDelegate(_entitySchema, info);
			((Func<IDataReader, Entity>)result)(_dataReader.Object);

			_dataReader.Verify(r => r.GetInt32(0));
			_dataReader.Verify(r => r.GetString(1));
			_dataReader.Verify(r => r.GetBoolean(2));
		}
		[Test]
		public void GenerateMaterializationDelegate_ParamsBasedMaterialization_ValidEntityCreated()
		{
			var expectedId = 23412;
			var expectedName = "description";
			var expectedIgnore = true;
			_dataReader.Setup(r => r.GetInt32(It.IsAny<int>())).Returns(expectedId);
			_dataReader.Setup(r => r.GetString(It.IsAny<int>())).Returns(expectedName);
			_dataReader.Setup(r => r.GetBoolean(It.IsAny<int>())).Returns(expectedIgnore);

			var info = new EntityMaterializationInformation(new[] { 0, 1, 3 });
			var builder = CreateBuilder();
			var result = builder.CreateEntityMaterializationDelegate(_entitySchema, info);
			var entity = ((Func<IDataReader, Entity>)result)(_dataReader.Object);

			Assert.AreEqual(expectedId, entity.Id);
			Assert.AreEqual(null, entity.Descr);
			Assert.AreEqual(expectedName, entity.Name);
			Assert.AreEqual(expectedIgnore, entity.Ignore);
		}

		[Test]
		public void GenerateMaterializationDelegate_ScalarMaterialization_ValidEntityCreated()
		{
			var val = "val";
			
			_dataReader.Setup(r => r.GetString(It.IsAny<int>())).Returns(val);

			var info = new EntityMaterializationInformation(typeof(string));
			var builder = CreateBuilder();
			var result = builder.CreateEntityMaterializationDelegate(new EntitySchema(typeof(string),
								new StorageName(false,"unknown",null),new Dictionary<string,StorageField>()), info);
			var entity = ((Func<IDataReader, string>)result)(_dataReader.Object);

			Assert.AreEqual(val, entity);
		}
		[Test]
		public void GenerateMaterializationDelegate_ScalarMaterialization_ValidDelegateGenerated()
		{
			var info = new EntityMaterializationInformation(typeof(string));
			var builder = CreateBuilder();
			var result = builder.CreateEntityMaterializationDelegate(new EntitySchema(typeof(string),
								new StorageName(false,"unknown", null), new Dictionary<string, StorageField>()), info);
			((Func<IDataReader, string>)result)(_dataReader.Object);

			_dataReader.Verify(r => r.GetString(0));
		}
		#endregion
		#region CreateInsertionParametersInitializerAction
		[Test]
		public void CreateInsertionParametersInitializer_ValidDelegateGenerated()
		{
			_command.Setup(c => c.CommandText).Returns("");
			var entitiesToInsert = new[]{
				new Entity(){Id = 13234, Descr = "d1", Ignore = false},
				new Entity(){Id = 111, Descr = null, Ignore = true}
			};
			var builder = CreateBuilder();
			builder.CreateParameterFactoryFunc = (n, v, t) => ParameterFactory(n, v, t);
			var info = new EntityInsertionInformation(new[]{0,2,3});
			var result = builder.CreateInsertionParametersInitializerAction(_entitySchema, info);
			result(_command.Object,entitiesToInsert);

			_command.VerifySet(c => c.CommandText = " VALUES (@p0, @p1, @p2), (@p3, NULL, @p4)");
			_parameterCollection.Verify(p => p.Add(It.Is<FakeParameter>(f => f.Name == "p0" && ((int)f.Value) == entitiesToInsert[0].Id && f.Type == _fieldType.Object)));
			_parameterCollection.Verify(p => p.Add(It.Is<FakeParameter>(f => f.Name == "p1" && ((string)f.Value) == entitiesToInsert[0].Descr && f.Type == null)));
			_parameterCollection.Verify(p => p.Add(It.Is<FakeParameter>(f => f.Name == "p2" && ((bool)f.Value) == entitiesToInsert[0].Ignore && f.Type == null)));

			_parameterCollection.Verify(p => p.Add(It.Is<FakeParameter>(f => f.Name == "p3" && ((int)f.Value) == entitiesToInsert[1].Id && f.Type == _fieldType.Object)));
			_parameterCollection.Verify(p => p.Add(It.Is<FakeParameter>(f => f.Value == null)),Times.Never());
			_parameterCollection.Verify(p => p.Add(It.Is<FakeParameter>(f => f.Name == "p4" && ((bool)f.Value) == entitiesToInsert[1].Ignore && f.Type == null)));
		}

		#endregion
		private FakeBuilder CreateBuilder()
		{
			return new FakeBuilder();
		}

		internal class FakeBuilder : DelegatesBuilder
		{
			public Func<Expression, Expression, IStorageFieldType, Expression> CreateParameterFactoryFunc { get; set; }

			protected override Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor,
			                                                     IStorageFieldType storageParameterType,
			                                                     ParameterDirection direction)
			{
				return CreateParameterFactoryFunc(parameterName, parameterAccessor, storageParameterType);
			}

			protected Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor,
			                                                     IStorageFieldType storageParameterType)
			{
				return CreateParameterFactory(parameterName, parameterAccessor, storageParameterType, ParameterDirection.Input);
			}



			protected override Expression CreateCommandReturnParameter(Type returnType, object dbType)
			{
				throw new NotImplementedException();
			}
		}
	}
}
