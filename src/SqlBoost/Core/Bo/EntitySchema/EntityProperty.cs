using System.Reflection;

namespace SqlBoost.Core.Bo.EntitySchema
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
