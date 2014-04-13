using System;
using System.Data;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.SchemaManager.EntitySchema;

namespace ObjectSql.Core.QueryBuilder.LambdaBuilder
{
	public interface IDelegatesBuilder
	{
		Func<IDbCommand, object> ReadCommandReturnParameter();
		Action<IDbCommand, QueryRoots> AddCommandReturnParameter(Type returnType,object dbType);
		Action<IDbCommand, QueryRoots> CreateDatabaseParameterFactoryAction(QueryRoots roots, Expression parameterName, Expression valueAccessor, IStorageFieldType parameterType, ParameterDirection direction);
		Action<IDbCommand, QueryRoots> CreateArrayParameters(QueryRoots roots, string paramName, Expression valueAccessor, IStorageFieldType parameterType, ParameterDirection direction);
		Action<IDbCommand, QueryRoots> CreateInsertionParametersInitializerAction(QueryRoots roots, EntitySchema entitySchema, EntityInsertionInformation insertionInfo);
		Action<IDbCommand, QueryRoots> CreateChangeDatabaseCommandTypeAction(CommandType commandType);
		Delegate CreateEntityMaterializationDelegate(EntitySchema schema, EntityMaterializationInformation materializationInfo);
		Action<IDbCommand, QueryRoots> CreateCommandParameterReader(QueryRoots queryRoots, ConstantExpression parameterName, Expression valueAccessor);
	}
}
