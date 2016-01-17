using ObjectSql.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSql.Core.SchemaManager.EntitySchema
{
	public class EntitySchema
	{
		private IDictionary<string, StorageField> _entityFieldsMap;

		public Type EntityType { get; private set; }
		public StorageName StorageName { get; private set; }
		public EntityProperty[] EntityProperties { get; private set; }
		public EntityProperty[] EntityFields { get; private set; }
		public StorageField[] StorageFields { get; private set; }

		private Func<object, int, object> _propertiesAccess;

		public EntitySchema(Type entityType, StorageName storageName, IDictionary<string, StorageField> entityToStorageFieldsMap)
		{
			EntityType = entityType;
			StorageName = storageName;
			_entityFieldsMap = entityToStorageFieldsMap;
			StorageFields = _entityFieldsMap.Values.ToArray();

			var entityProperties = entityType.GetProps().ToArray();
			
			EntityProperties = new EntityProperty[entityProperties.Length];
			for(int i = 0;i<entityProperties.Length;i++)
			{
				var storageField = entityToStorageFieldsMap.Where(x=>x.Key == entityProperties[i].Name)
					.Select(x=>x.Value).FirstOrDefault();
				EntityProperties[i] = new EntityProperty(entityProperties[i], i, storageField);
			}
			EntityFields = EntityProperties.Where(p => p.Mapped).ToArray();

			_propertiesAccess = CreatePropertyAccessDelegate(entityType, EntityProperties);
		}
		public EntityProperty GetEntityPropertyByName(string entityProperty)
		{
			return EntityProperties.Single(p => p.Name == entityProperty);
		}
		public EntityProperty GetEntityPropertyByIndex(int entityPropertyIndex)
		{
			return EntityProperties.Single(p => p.Index == entityPropertyIndex);
		}
		public StorageField GetStorageField(string entityField)
		{
			return _entityFieldsMap[entityField];
		}
		public object GetEntityPropertyValue<TEntity>(TEntity val, EntityProperty entityProperty)
		{
			return _propertiesAccess(val, entityProperty.Index);
		}
		private Func<object, int, object> CreatePropertyAccessDelegate(Type entity,
			EntityProperty[] entityProperties)
		{
			if (entityProperties.Length == 0)
				return null;
			var entityParam = LambdaExpression.Parameter(typeof(object));
			var fieldIndexParam = LambdaExpression.Parameter(typeof(int));

			var switchCases = new SwitchCase[entityProperties.Length];
			for (int i = 0;i<switchCases.Length;i++)
			{
				switchCases[i] = LambdaExpression.SwitchCase(
									LambdaExpression.Convert(
										LambdaExpression.MakeMemberAccess(
											LambdaExpression.Convert(entityParam, entity),
											entity.GetProps()[i]),
										typeof(object)),
									LambdaExpression.Constant(i));
			}

			return LambdaExpression.Lambda<Func<object, int, object>>(
				LambdaExpression.Switch(fieldIndexParam, LambdaExpression.Constant(null,typeof(object)), switchCases),
				entityParam,fieldIndexParam)
				.Compile();
		}

	}
}
