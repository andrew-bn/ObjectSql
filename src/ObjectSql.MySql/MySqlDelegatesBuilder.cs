using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using MySql.Data.MySqlClient;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager.EntitySchema;

namespace ObjectSql.MySql
{
	public class MySqlDelegatesBuilder : DelegatesBuilder
	{
		public static MySqlDelegatesBuilder Instance = new MySqlDelegatesBuilder();

		private MySqlDelegatesBuilder()
		{
		}

		protected override Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor,
			IStorageFieldType storageParameterType, ParameterDirection direction)
		{
			var dbType = !(storageParameterType is StorageFieldType<MySqlDbType> fieldType)
								? null 
								: (object) fieldType.Value;

			return CreateParameterFactory(parameterName, parameterAccessor, direction, dbType);
		}

		private static Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor,
			ParameterDirection direction, object fieldType)
		{
			Expression parameterCreate;
			if (fieldType != null)
			{
				parameterCreate = Expression.New(Reflect.FindCtor(() => new MySqlParameter("", default(MySqlDbType))),
					parameterName,
					Expression.Constant((MySqlDbType) fieldType));
				parameterCreate = Expression.MemberInit((NewExpression) parameterCreate,
					Expression.Bind(Reflect.FindProperty<MySqlParameter>(p => p.Value),
						parameterAccessor),
					Expression.Bind(Reflect.FindProperty<MySqlParameter>(p => p.Direction),
						Expression.Constant(direction)));
			}
			else
			{
				parameterCreate = Expression.New(Reflect.FindCtor(() => new MySqlParameter("", default(object))),
					parameterName,
					parameterAccessor);
			}

			return parameterCreate;
		}

		protected override Expression CreateCommandReturnParameter(Type returnType, object dbType)
		{
			return CreateParameterFactory(Expression.Constant(ReturnParameterName),
				Expression.Convert(Expression.Default(returnType), typeof(object)),
				ParameterDirection.ReturnValue, dbType);
		}

		protected override string ReturnParameterName => "__l_return_l__";
	}
}
