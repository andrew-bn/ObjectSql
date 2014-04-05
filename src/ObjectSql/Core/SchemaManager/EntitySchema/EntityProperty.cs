using System.Reflection;

namespace ObjectSql.Core.SchemaManager.EntitySchema
{
	public class EntityProperty : EntityMember
	{
		public PropertyInfo PropertyInfo { get; private set; }
		public EntityProperty(PropertyInfo propertyInfo, int index, StorageField storageField)
			:base(propertyInfo.Name,index,storageField)
		{
			PropertyInfo = propertyInfo;
		}
	}
}
