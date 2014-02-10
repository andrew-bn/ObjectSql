using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using SqlBoost.Core.Bo.EntitySchema;

namespace SqlBoost.Core.SchemaManager
{
	internal class EntitySchemaManager<TTypeEnum> : IEntitySchemaManager
		where TTypeEnum : struct
	{
		readonly ConcurrentDictionary<Type, EntitySchema> _schemas = new ConcurrentDictionary<Type, EntitySchema>();
		readonly ConcurrentDictionary<MethodInfo, FuncSchema> _funcSchemas = new ConcurrentDictionary<MethodInfo, FuncSchema>();

		public StorageName StorageName(Type entityType)
		{
			var entitySchema = GetSchema(entityType);
			return entitySchema.StorageName;
		}

		public EntitySchema GetSchema(Type entityType)
		{
			return _schemas.GetOrAdd(entityType, CreateSchema);
		}

		public FuncSchema GetFuncSchema(MethodInfo method)
		{
			return _funcSchemas.GetOrAdd(method, CreateFuncSchema);
		}

		protected virtual FuncSchema CreateFuncSchema(MethodInfo method)
		{
			int index = 0;
			var parameters = method.GetParameters().Select(
				p => new FuncParameter(p.Name, index++, new StorageField(p.Name, null))).ToArray();
			return new FuncSchema(new StorageName(method.Name, ""), parameters);
		}

		#region entitySchema
		protected virtual EntitySchema CreateSchema(Type entity)
		{
			var entityFields = entity.GetProperties()
									.Where(NotFilteredEntityProperty)
									.ToArray();

			return new EntitySchema(
				entity,
				ObtainStorageName(entity),
				entityFields.ToDictionary(f => f.Name, ObtainStorageField));
		}

		private bool NotFilteredEntityProperty(PropertyInfo prop)
		{
			var notMappedAttr = prop.GetCustomAttribute(typeof(NotMappedAttribute)) as NotMappedAttribute;
			return notMappedAttr == null;
		}
		private StorageField ObtainStorageField(PropertyInfo prop)
		{
			var annotationAttribute = prop.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
			return annotationAttribute == null
					? new StorageField(prop.Name)
					: new StorageField(annotationAttribute.Name, ParseDbType(annotationAttribute.TypeName));
		}
		private StorageName ObtainStorageName(Type entity)
		{
			var annotationAttribute = entity.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
			return annotationAttribute == null
					? new StorageName(entity.Name, String.Empty)
					: new StorageName(annotationAttribute.Name, annotationAttribute.Schema);
		}
		#endregion
		protected IStorageFieldType ParseDbType(string value)
		{
			TTypeEnum result;
			return Enum.TryParse(value, true, out result)
					? new StorageFieldType<TTypeEnum>(result)
					: null;
		}
	}
}
