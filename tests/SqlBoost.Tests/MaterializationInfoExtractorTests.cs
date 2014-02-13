﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlBoost.Core.Bo;
using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core.Misc;
using SqlBoost.Core.QueryBuilder.InfoExtractor;
using SqlBoost.Core.SchemaManager;
using SqlBoost.QueryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SqlBoost;
namespace SqlBoost.Tests
{
	[TestClass]
	public class MaterializationInfoExtractorTests
	{
		private Mock<IEntitySchemaManager> _schemaManager;

		private EntitySchema _categorySchema;
		private string _categoryNameField;
		public class Dto
		{
			public Dto() { }
			public Dto(int identity, string dtoName) { }
			public int Id { get; set; }
			public string Name { get; set; }
		}
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
		}
		[TestMethod]
		public void ExtractFrom_ParameterMaterialization()
		{
			Expression<Func<Category, object>> exp = c => c;
			var extractor = CreateExtractor();
			var result = extractor.ExtractFrom(exp.Body);

			Assert.IsFalse(result.IsConstructorBased);
			Assert.IsFalse(result.IsSingleValue);
			Assert.IsTrue(result.FieldsIndexes.SequenceEqual(new[] { 0, 1, 2, 3 }));
		}
		[TestMethod]
		public void ExtractFrom_SingleValueMaterialization()
		{
			Expression<Func<Category,object>> exp = (c) => c.Picture;
			var extractor = CreateExtractor();
			var result = extractor.ExtractFrom(exp.Body);

			Assert.IsFalse(result.IsConstructorBased);
			Assert.IsTrue(result.IsSingleValue);
			Assert.AreEqual(typeof(byte[]),result.SingleValueType);
		}
		[TestMethod]
		public void ExtractFrom_MethodCallResultMaterialization()
		{
			Expression<Func<Category, object>> exp = (c) => Sql.Count(c.CategoryID);
			var extractor = CreateExtractor();
			var result = extractor.ExtractFrom(exp.Body);

			Assert.IsFalse(result.IsConstructorBased);
			Assert.IsTrue(result.IsSingleValue);
			Assert.AreEqual(typeof(int), result.SingleValueType);
		}
		[TestMethod]
		public void ExtractFrom_ConstructorMaterialization()
		{
			Expression<Func<Category, object>> exp = (c) => new Dto(c.CategoryID, c.CategoryName);
			var extractor = CreateExtractor();
			var result = extractor.ExtractFrom(exp.Body);

			Assert.IsTrue(result.IsConstructorBased);
			Assert.IsFalse(result.IsSingleValue);
			Assert.AreEqual(Reflect.FindCtor(()=>new Dto(1,"")), result.ConstructorInfo);
		}
		[TestMethod]
		public void ExtractFrom_MemberInitMaterialization()
		{
			_categorySchema = new EntitySchema(typeof(Dto), new StorageName("Dto", null),
									new Dictionary<string, StorageField>()
									{
										{ "Id", new StorageField("Id", null) }, 
										{ "Name", new StorageField("Name", null) }, 
									});
			_schemaManager.Setup(m => m.GetSchema(It.IsAny<Type>())).Returns(_categorySchema);

			Expression<Func<Category, object>> exp = (c) => new Dto() { Name = c.Description};
			var extractor = CreateExtractor();
			var result = extractor.ExtractFrom(exp.Body);

			Assert.IsFalse(result.IsConstructorBased);
			Assert.IsFalse(result.IsSingleValue);
			Assert.IsTrue(result.FieldsIndexes.SequenceEqual(new[] { 1 }));
		}
		private MaterializationInfoExtractor CreateExtractor()
		{
			return new MaterializationInfoExtractor(_schemaManager.Object);
		}
	}
}
