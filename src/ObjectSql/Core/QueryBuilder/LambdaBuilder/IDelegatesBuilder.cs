using System;
using System.Data;
using System.Linq.Expressions;
using ObjectSql.Core.SchemaManager.EntitySchema;

namespace ObjectSql.Core.QueryBuilder.LambdaBuilder
{
	public interface IDelegatesBuilder
	{
		Func<IDbCommand, object> ReadCommandReturnParameter();
		Action<IDbCommand, object> AddCommandReturnParameter(Type returnType,object dbType);
		Action<IDbCommand, object> CreateDatabaseParameterFactoryAction(Expression parameterName, Expression valueAccessor, IStorageFieldType parameterType,ParameterDirection direction);
		Action<IDbCommand, object> CreateArrayParameters(string paramName, Expression valueAccessor, IStorageFieldType parameterType, ParameterDirection direction);
		Action<IDbCommand, object> CreateInsertionParametersInitializerAction(EntitySchema entitySchema, EntityInsertionInformation insertionInfo);
		Action<IDbCommand, object> CreateChangeDatabaseCommandTypeAction(CommandType commandType);
		Delegate CreateEntityMaterializationDelegate(EntitySchema schema, EntityMaterializationInformation materializationInfo);
		Action<IDbCommand, object> CreateCommandParameterReader(ConstantExpression parameterName, Expression valueAccessor);
	}
}
