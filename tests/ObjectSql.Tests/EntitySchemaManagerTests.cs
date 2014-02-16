using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Core.Bo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectSql.Tests
{
	[TestClass]
	public class EntitySchemaManagerTests
	{
		public class Entity
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
		}
		[TestMethod]
		public void GetSchema_NoAnnotations_ReturnsValidSchema()
		{
			var mng = CreateManager();
			var result = mng.GetSchema(typeof(Entity));

			Assert.AreEqual("Entity", result.StorageName.Name);
			Assert.IsTrue(string.IsNullOrEmpty(result.StorageName.Schema));

			Assert.AreEqual(2, result.EntityProperties.Length);
			Assert.AreEqual(2, result.EntityFields.Length);
			Assert.AreEqual(2, result.StorageFields.Length);

			Assert.AreEqual("Id", result.EntityProperties[0].Name);
			Assert.AreEqual(0, result.EntityProperties[0].Index);
			Assert.AreEqual("Id", result.EntityProperties[0].StorageField.Name);
			Assert.AreEqual(null, result.EntityProperties[0].StorageField.DbType);
			Assert.IsTrue(result.EntityProperties[0].Mapped);

			Assert.AreEqual("Name", result.EntityProperties[1].Name);
			Assert.AreEqual(1, result.EntityProperties[1].Index);
			Assert.AreEqual("Name", result.EntityProperties[1].StorageField.Name);
			Assert.AreEqual(null, result.EntityProperties[1].StorageField.DbType);
			Assert.IsTrue(result.EntityProperties[1].Mapped);
		}
		[Table("Entity table",Schema="someschema")]
		public class Entity2
		{
			[Column("Identity",TypeName="uniqueidentifier")]
			public Guid Id { get; set; }
			[Column("Name",TypeName="NChar")]
			public string Name { get; set; }
			[NotMapped]
			public bool Ignored{get;set;}
		}
		[TestMethod]
		public void GetSchema_WithAnnotations_ReturnsValidSchema()
		{
			var mng = CreateManager();
			var result = mng.GetSchema(typeof(Entity2));

			Assert.AreEqual("Entity table", result.StorageName.Name);
			Assert.AreEqual("someschema",result.StorageName.Schema);

			Assert.AreEqual(3, result.EntityProperties.Length);
			Assert.AreEqual(2, result.EntityFields.Length);
			Assert.AreEqual(2, result.StorageFields.Length);

			#region properties
			Assert.AreEqual("Id", result.EntityProperties[0].Name);
			Assert.AreEqual(0, result.EntityProperties[0].Index);
			Assert.AreEqual("Identity", result.EntityProperties[0].StorageField.Name);
			Assert.AreEqual(SqlDbType.UniqueIdentifier, ((StorageFieldType<SqlDbType>)result.EntityProperties[0].StorageField.DbType).Value);
			Assert.IsTrue(result.EntityProperties[0].Mapped);

			Assert.AreEqual("Name", result.EntityProperties[1].Name);
			Assert.AreEqual(1, result.EntityProperties[1].Index);
			Assert.AreEqual("Name", result.EntityProperties[1].StorageField.Name);
			Assert.AreEqual(SqlDbType.NChar, ((StorageFieldType<SqlDbType>)result.EntityProperties[1].StorageField.DbType).Value);
			Assert.IsTrue(result.EntityProperties[1].Mapped);

			Assert.AreEqual("Ignored", result.EntityProperties[2].Name);
			Assert.AreEqual(2, result.EntityProperties[2].Index);
			Assert.IsNull(result.EntityProperties[2].StorageField);
			Assert.IsFalse(result.EntityProperties[2].Mapped);
			#endregion
		}
		private IEntitySchemaManager CreateManager()
		{
			return new EntitySchemaManager<SqlDbType>();
		}
	}
}
