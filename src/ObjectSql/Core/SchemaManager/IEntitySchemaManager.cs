using System;
using System.Reflection;
using ObjectSql.Core.Bo.EntitySchema;

namespace ObjectSql.Core.SchemaManager
{
	public interface IEntitySchemaManager
	{
		EntitySchema GetSchema(Type entityType);
		FuncSchema GetFuncSchema(MethodInfo method);
	}
}
