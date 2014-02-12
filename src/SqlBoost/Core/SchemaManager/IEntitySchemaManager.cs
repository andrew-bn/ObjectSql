using System;
using System.Reflection;
using SqlBoost.Core.Bo.EntitySchema;

namespace SqlBoost.Core.SchemaManager
{
	public interface IEntitySchemaManager
	{
		EntitySchema GetSchema(Type entityType);
		FuncSchema GetFuncSchema(MethodInfo method);
	}
}
