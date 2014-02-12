using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core.Misc;
using SqlBoost.Core.QueryBuilder.LambdaBuilder;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace SqlBoost.SqlServer
{
	internal class SqlServerDelegatesBuilder: DelegatesBuilder
	{
		protected override Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor, IStorageFieldType storageParameterType)
		{
			var fieldType = storageParameterType as StorageFieldType<SqlDbType>;
			Expression parameterCreate;
			if (fieldType != null)
			{
				parameterCreate = Expression.New(Reflect.FindCtor(() => new SqlParameter("", default(SqlDbType))),
												parameterName,
												Expression.Constant(fieldType.Value));
				parameterCreate = Expression.MemberInit((NewExpression)parameterCreate,
									Expression.Bind(Reflect.FindProperty<SqlParameter>(p => p.Value),
												Expression.Convert(parameterAccessor, typeof(object))));
			}
			else
			{
				parameterCreate = Expression.New(Reflect.FindCtor(() => new SqlParameter("", default(object))),
												parameterName,
												Expression.Convert(parameterAccessor, typeof(object)));
			}
			return parameterCreate;
		}
	}
}
