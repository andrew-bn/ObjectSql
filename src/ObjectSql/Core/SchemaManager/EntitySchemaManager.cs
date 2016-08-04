using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using ObjectSql.Core.SchemaManager.EntitySchema;
using System.Reflection;
using ObjectSql.Core.Misc;
namespace ObjectSql.Core.SchemaManager
{
	public class EntitySchemaManager<TTypeEnum> : IEntitySchemaManager
		where TTypeEnum : struct
	{
		readonly ConcurrentDictionary<Type, EntitySchema.EntitySchema> _schemas = new ConcurrentDictionary<Type, EntitySchema.EntitySchema>();
		readonly ConcurrentDictionary<MethodInfo, FuncSchema> _funcSchemas = new ConcurrentDictionary<MethodInfo, FuncSchema>();

		public StorageName StorageName(Type entityType)
		{
			var entitySchema = GetSchema(entityType);
			return entitySchema.StorageName;
		}

		public EntitySchema.EntitySchema GetSchema(Type entityType)
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
				p => new FuncParameter(p.Name, index++, ObtainStorageParameter(p))).ToArray();

			return new FuncSchema(ObtainStorageProcedureName(method), parameters);
		}

		#region entitySchema
		protected virtual EntitySchema.EntitySchema CreateSchema(Type entity)
		{
			var entityFields =  entity.GetProperties()
									.Where(NotFilteredEntityProperty)
									.ToArray();

			return new EntitySchema.EntitySchema(
				entity,
				ObtainStorageName(entity),
				entityFields.ToDictionary(f => f.Name, ObtainStorageField));
		}

		private bool NotFilteredEntityProperty(PropertyInfo prop)
		{
			object notMappedAttr = null;
			if (notMappedAttr == null)
				notMappedAttr = prop.GetCustomAttr(typeof(NotMappedAttribute)) as NotMappedAttribute;

			return notMappedAttr == null;
		}

		private StorageField ObtainStorageField(PropertyInfo prop)
		{
			string entityName = prop.Name;
			string dbType = string.Empty;
			bool attrFound = false;

			var objSqlAttr = prop.GetCustomAttr(typeof(ColumnAttribute)) as ColumnAttribute;
			if (attrFound = (objSqlAttr != null))
			{
				entityName = objSqlAttr.Name;
				dbType = objSqlAttr.TypeName;
			}

			return string.IsNullOrEmpty(dbType)
					? new StorageField(entityName)
					: new StorageField(entityName, ParseDbType(dbType));
		}

		private StorageName ObtainStorageName(Type entity)
		{
			string entityName = entity.Name;
			string schemaName = string.Empty;
			var nameOnly = false;
			bool attrFound = false;

			var objSqlAttr = entity.GetCustomAttr(typeof(TableAttribute)) as TableAttribute;
			if (attrFound = (objSqlAttr != null))
			{
				entityName = objSqlAttr.Name;
				schemaName = objSqlAttr.Schema;
			}

			return new StorageName(nameOnly, entityName, schemaName);
		}
		private StorageName ObtainStorageProcedureName(MethodInfo entity)
		{
			var attr = entity.GetCustomAttr(typeof(ProcedureAttribute)) as ProcedureAttribute;
			return attr == null
					? new StorageName(false, entity.Name, String.Empty)
					: new StorageName(false, attr.Name, attr.Schema);

		}
		private StorageParameter ObtainStorageParameter(ParameterInfo prop)
		{
			var attr = prop.GetCustomAttr(typeof(ParameterAttribute)) as ParameterAttribute;
			return attr == null
					? new StorageParameter(prop.Name, ParameterDirection.Input)
					: new StorageParameter(attr.Name, ParseDbType(attr.TypeName), attr.Direction);
		}
		#endregion
		public IStorageFieldType ParseDbType(string value)
		{
			
			TTypeEnum result;
			return Enum.TryParse(value, true, out result)
					? new StorageFieldType<TTypeEnum>(result)
					: null;
		}
	}
}
