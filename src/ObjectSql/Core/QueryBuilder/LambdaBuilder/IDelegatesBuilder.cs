using ObjectSql.Core.Bo.EntitySchema;
using System;
using System.Data;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.LambdaBuilder
{
	public interface IDelegatesBuilder
	{
		Action<IDbCommand, object> CreateDatabaseParameterFactoryAction(Expression parameterName, Expression valueAccessor, IStorageFieldType parameterType);
		Action<IDbCommand, object> CreateInsertionParametersInitializerAction(EntitySchema entitySchema, EntityInsertionInformation insertionInfo);
		Action<IDbCommand, object> CreateChangeDatabaseCommandTypeAction(CommandType commandType);
		Delegate CreateEntityMaterializationDelegate(EntitySchema schema, EntityMaterializationInformation materializationInfo);
		
	}
}
