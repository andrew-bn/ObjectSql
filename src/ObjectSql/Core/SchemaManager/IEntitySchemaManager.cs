using System;
using System.Reflection;
using ObjectSql.Core.SchemaManager.EntitySchema;

namespace ObjectSql.Core.SchemaManager
{
	public interface IEntitySchemaManager
	{
		EntitySchema.EntitySchema GetSchema(Type entityType);
		FuncSchema GetFuncSchema(MethodInfo method);
		IStorageFieldType ParseDbType(string value);
	}
}
