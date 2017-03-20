using System;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.SchemaManager.EntitySchema;

namespace ObjectSql.Core.QueryBuilder.LambdaBuilder
{
	public interface IDelegatesBuilder
	{
		Func<DbCommand, object> ReadCommandReturnParameter();
		Action<DbCommand, QueryRoots> AddCommandReturnParameter(Type returnType,object dbType);
		Action<DbCommand, QueryRoots> CreateDatabaseParameterFactoryAction(QueryRoots roots, Expression parameterName, Expression valueAccessor, IStorageFieldType parameterType, ParameterDirection direction);
		Action<DbCommand, QueryRoots> CreateArrayParameters(QueryRoots roots, string paramName, Expression valueAccessor, IStorageFieldType parameterType, ParameterDirection direction);
		Action<DbCommand, QueryRoots> CreateInsertionParametersInitializerAction(QueryRoots roots, EntitySchema entitySchema, EntityInsertionInformation insertionInfo, string placeHolder);
		Action<DbCommand, QueryRoots> CreateChangeDatabaseCommandTypeAction(CommandType commandType);
		Delegate CreateEntityMaterializationDelegate(EntitySchema schema, EntityMaterializationInformation materializationInfo);
		Action<DbCommand, QueryRoots> CreateCommandParameterReader(QueryRoots queryRoots, ConstantExpression parameterName, Expression valueAccessor);
	}
}
