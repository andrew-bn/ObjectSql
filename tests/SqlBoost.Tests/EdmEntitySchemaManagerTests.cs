using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlBoost.Core.Bo;
using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core.SchemaManager;
using SqlBoost.EF5;
using SqlBoost.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBoost.Tests
{
	[TestClass]
	public class EdmEntitySchemaManagerTests
	{
		[TestMethod]
		public void GetSchema_ReturnsValidSchema()
		{
			var manager = CreateManager();
			var result = manager.GetSchema(typeof(Category));

			Assert.AreEqual("Categories", result.StorageName.Name);
			Assert.AreEqual("dbo", result.StorageName.Schema);

			Assert.AreEqual(5, result.EntityProperties.Length);
			Assert.AreEqual(4, result.EntityFields.Length);
			Assert.AreEqual(4, result.StorageFields.Length);

			#region properties
			Assert.AreEqual("CategoryID", result.EntityProperties[0].Name);
			Assert.AreEqual(0, result.EntityProperties[0].Index);
			Assert.AreEqual("CategoryID", result.EntityProperties[0].StorageField.Name);
			Assert.AreEqual(SqlDbType.Int, ((StorageFieldType<SqlDbType>)result.EntityProperties[0].StorageField.DbType).Value);
			Assert.IsTrue(result.EntityProperties[0].Mapped);

			Assert.AreEqual("CategoryName", result.EntityProperties[1].Name);
			Assert.AreEqual(1, result.EntityProperties[1].Index);
			Assert.AreEqual("CategoryName", result.EntityProperties[1].StorageField.Name);
			Assert.AreEqual(SqlDbType.NVarChar, ((StorageFieldType<SqlDbType>)result.EntityProperties[1].StorageField.DbType).Value);
			Assert.IsTrue(result.EntityProperties[1].Mapped);

			Assert.AreEqual("Description", result.EntityProperties[2].Name);
			Assert.AreEqual(2, result.EntityProperties[2].Index);
			Assert.AreEqual("Description", result.EntityProperties[2].StorageField.Name);
			Assert.AreEqual(SqlDbType.NText, ((StorageFieldType<SqlDbType>)result.EntityProperties[2].StorageField.DbType).Value);
			Assert.IsTrue(result.EntityProperties[2].Mapped);

			Assert.AreEqual("Picture", result.EntityProperties[3].Name);
			Assert.AreEqual(3, result.EntityProperties[3].Index);
			Assert.AreEqual("Picture", result.EntityProperties[3].StorageField.Name);
			Assert.AreEqual(SqlDbType.Image, ((StorageFieldType<SqlDbType>)result.EntityProperties[3].StorageField.DbType).Value);
			Assert.IsTrue(result.EntityProperties[3].Mapped);

			Assert.AreEqual("Products", result.EntityProperties[4].Name);
			Assert.AreEqual(4, result.EntityProperties[4].Index);
			Assert.IsNull(result.EntityProperties[4].StorageField);
			Assert.IsFalse(result.EntityProperties[4].Mapped);
			#endregion
		}
		[TestMethod]
		[ExpectedException(typeof(SqlBoostException))]
		public void CreateManager_InvalidCsdlNameDescriptor_ExceptionExpected()
		{
			new EdmEntitySchemaManager<SqlDbType>("res://*/INVALIDNAME.csdl|res://*/Northwind.ssdl|res://*/Northwind.msl");
		}
		[TestMethod]
		[ExpectedException(typeof(SqlBoostException))]
		public void CreateManager_InvalidMetadataDescriptor_ExceptionExpected()
		{
			new EdmEntitySchemaManager<SqlDbType>("res://*/InvalidMetadataDescriptorFormat:/*/Northwind.msl");
		}
		private IEntitySchemaManager CreateManager()
		{
			return new EdmEntitySchemaManager<SqlDbType>(@"metadata=res://*/Northwind.csdl|res://*/Northwind.ssdl|res://*/Northwind.msl;provider=System.Data.SqlClient;provider connection string=""data source=(LocalDB)\v11.0;attachdbfilename=D:\Work\Git\ObjectSql\tests\SqlBoost.Tests\TestDatabase.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework""");
		}
	}
}
