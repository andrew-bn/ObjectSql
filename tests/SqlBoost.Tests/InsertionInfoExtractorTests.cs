using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SqlBoost.Core.Bo;
using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core.QueryBuilder.InfoExtractor;
using SqlBoost.Core.SchemaManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost.Tests
{
	[TestClass]
	public class InsertionInfoExtractorTests
	{
		private Mock<IEntitySchemaManager> _schemaManager;

		private EntitySchema _categorySchema;
		private string _categoryNameField;
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
		public void ExtractFrom_ParameterInsertionSchema()
		{
			Expression<Func<Category, object>> exp = c => c;
			var extractor = CreateExtractor();
			var result = extractor.ExtractFrom(exp.Body);

			Assert.AreEqual(4, result.PropertiesIndexesToInsert.Length);
			Assert.IsTrue(result.PropertiesIndexesToInsert.SequenceEqual(new[]{0,1,2,3}));
		}
		[TestMethod]
		public void ExtractFrom_FieldSequenseInsertionSchema()
		{
			Expression<Func<Category, object>> exp = c => new { c.Picture,c.CategoryName };
			var extractor = CreateExtractor();
			var result = extractor.ExtractFrom(exp.Body);

			Assert.AreEqual(2, result.PropertiesIndexesToInsert.Length);
			Assert.IsTrue(result.PropertiesIndexesToInsert.SequenceEqual(new[] {  3,1 }));
		}
		private InsertionInfoExtractor CreateExtractor()
		{
			return new InsertionInfoExtractor(_schemaManager.Object);
		}
	}
}
