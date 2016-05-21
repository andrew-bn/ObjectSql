//using Moq;
//using ObjectSql.Core.QueryBuilder.InfoExtractor;
//using ObjectSql.Core.SchemaManager;
//using ObjectSql.Core.Bo;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using ObjectSql.Core.SchemaManager.EntitySchema;
//using Xunit;
//using ObjectSql.Test.Database.TestDatabase.dbo;

//namespace ObjectSql.Tests
//{
//	public class InsertionInfoExtractorTests
//	{
//		private Mock<IEntitySchemaManager> _schemaManager;

//		private EntitySchema _categorySchema;
//		private string _categoryNameField;
//		public InsertionInfoExtractorTests()
//		{
//			_categoryNameField = "Category Name Fld";
//			_categorySchema = new EntitySchema(typeof(Categories), new StorageName(false, "Category", null),
//									new Dictionary<string, StorageField>()
//									{
//										{ "CategoryID", new StorageField("CategoryID", null) },
//										{ "CategoryName", new StorageField(_categoryNameField, null) },
//										{ "Description", new StorageField("Description", null) },
//										{ "Picture", new StorageField("Picture", null) },
//									});
//			_schemaManager = new Mock<IEntitySchemaManager>();
//			_schemaManager.Setup(m => m.GetSchema(It.IsAny<Type>())).Returns(_categorySchema);
//		}
//		[Fact]
//		public void ExtractFrom_ParameterInsertionSchema()
//		{
//			Expression<Func<Categories, object>> exp = c => c;
//			var extractor = CreateExtractor();
//			var result = extractor.ExtractFrom(exp.Body);

//			Assert.Equal(4, result.PropertiesIndexesToInsert.Length);
//			Assert.True(result.PropertiesIndexesToInsert.SequenceEqual(new[] { 0, 1, 2, 3 }));
//		}
//		[Fact]
//		public void ExtractFrom_FieldSequenseInsertionSchema()
//		{
//			Expression<Func<Categories, object>> exp = c => new { c.Picture, c.CategoryName };
//			var extractor = CreateExtractor();
//			var result = extractor.ExtractFrom(exp.Body);

//			Assert.Equal(2, result.PropertiesIndexesToInsert.Length);
//			Assert.True(result.PropertiesIndexesToInsert.SequenceEqual(new[] { 3, 1 }));
//		}
//		private InsertionInfoExtractor CreateExtractor()
//		{
//			return new InsertionInfoExtractor(_schemaManager.Object);
//		}
//	}
//}
