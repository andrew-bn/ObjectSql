using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace ObjectSql.SqlServer
{
	public class SqlServerDelegatesBuilder: DelegatesBuilder
	{
		public static SqlServerDelegatesBuilder Instance = new SqlServerDelegatesBuilder();
		private SqlServerDelegatesBuilder(){}
		protected override Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor, IStorageFieldType storageParameterType, ParameterDirection direction)
		{
			var fieldType = storageParameterType as StorageFieldType<SqlDbType>;
			return CreateParameterFactory(parameterName, parameterAccessor, direction, fieldType.Value);
		}

		private static Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor,
		                                                 ParameterDirection direction, object fieldType)
		{
			Expression parameterCreate;
			if (fieldType != null)
			{
				parameterCreate = Expression.New(Reflect.FindCtor(() => new SqlParameter("", default(SqlDbType))),
				                                 parameterName,
												 Expression.Constant((SqlDbType)fieldType));
				parameterCreate = Expression.MemberInit((NewExpression) parameterCreate,
				                                        Expression.Bind(Reflect.FindProperty<SqlParameter>(p => p.Value),
				                                                        parameterAccessor),
				                                        Expression.Bind(Reflect.FindProperty<SqlParameter>(p => p.Direction),
				                                                        Expression.Constant(direction)));
			}
			else
			{
				parameterCreate = Expression.New(Reflect.FindCtor(() => new SqlParameter("", default(object))),
				                                 parameterName,
				                                 parameterAccessor);
			}

			return parameterCreate;
		}

		protected override Expression CreateCommandReturnParameter(System.Type returnType, object dbType)
		{
			return CreateParameterFactory(Expression.Constant("__l_return_l__"), Expression.Convert(Expression.Default(returnType),typeof(object)),
			                              ParameterDirection.ReturnValue, dbType);
		}
	}
}
