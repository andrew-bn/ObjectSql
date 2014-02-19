using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Reflection;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.Misc;

namespace ObjectSql.Core.SchemaManager
{
	public class EntitySchemaManager<TTypeEnum> : IEntitySchemaManager
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
				p => new FuncParameter(p.Name, index++, ObtainStorageParameter(p))).ToArray();

			return new FuncSchema(ObtainStorageProcedureName(method), parameters);
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
			object notMappedAttr = null;
#if NET45
			notMappedAttr = prop.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute)) as System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute;
#endif
			if (notMappedAttr == null)
				notMappedAttr = prop.GetCustomAttribute(typeof(NotMappedAttribute)) as NotMappedAttribute;

			return notMappedAttr == null;
		}

		private StorageField ObtainStorageField(PropertyInfo prop)
		{
			string entityName = prop.Name;
			string dbType = string.Empty;
			bool attrFound = false;

			var objSqlAttr = prop.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
			if (attrFound = (objSqlAttr != null))
			{
				entityName = objSqlAttr.Name;
				dbType = objSqlAttr.TypeName;
			}

#if NET45
			if (!attrFound)
			{
				var netAttr = prop.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)) as System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;
				if (netAttr != null)
				{
					entityName = netAttr.Name;
					dbType = netAttr.TypeName;
				}
			}
#endif
#if NET40
			if (!attrFound)
			{
				var netAttr = prop.GetCustomAttribute(typeof(System.Data.Linq.Mapping.ColumnAttribute)) as System.Data.Linq.Mapping.ColumnAttribute;
				if (netAttr != null)
				{
					entityName = netAttr.Name;
					dbType = netAttr.DbType;
				}
			}
#endif
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

			var objSqlAttr = entity.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
			if (attrFound = (objSqlAttr != null))
			{
				entityName = objSqlAttr.Name;
				schemaName = objSqlAttr.Schema;
			}

#if NET45
			if (!attrFound)
			{
				var netAttr = entity.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute)) as System.ComponentModel.DataAnnotations.Schema.TableAttribute;
				if (netAttr != null)
				{
					entityName = netAttr.Name;
					schemaName = netAttr.Schema;
				}
			}
#endif
#if NET40
			if (!attrFound)
			{
				var netAttr = entity.GetCustomAttribute(typeof(System.Data.Linq.Mapping.TableAttribute)) as System.Data.Linq.Mapping.TableAttribute;
				if (netAttr != null)
				{
					entityName = netAttr.Name;
					nameOnly = true;
				}
			}
#endif
			return new StorageName(nameOnly, entityName, schemaName);
		}
		private StorageName ObtainStorageProcedureName(MethodInfo entity)
		{
			var attr = entity.GetCustomAttribute(typeof(ProcedureAttribute)) as ProcedureAttribute;
			return attr == null
					? new StorageName(false, entity.Name, String.Empty)
					: new StorageName(false, attr.Name, attr.Schema);

		}
		private StorageParameter ObtainStorageParameter(ParameterInfo prop)
		{
			var attr = prop.GetCustomAttribute(typeof(ParameterAttribute)) as ParameterAttribute;
			return attr == null
					? new StorageParameter(prop.Name, ParameterDirection.Input)
					: new StorageParameter(attr.Name, ParseDbType(attr.TypeName), attr.Direction);
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
