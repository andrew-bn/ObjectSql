using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.Misc;
using ObjectSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectSql.Core.QueryBuilder.LambdaBuilder
{
	public abstract class DelegatesBuilder : IDelegatesBuilder
	{
		public Action<IDbCommand, object> CreateDatabaseParameterFactoryAction(Expression parameterName, Expression valueAccessor, IStorageFieldType parameterType)
		{
			var expQueue = new Stack<Expression>(ExpressionEnumerator.Enumerate(valueAccessor).Cast<Expression>().ToArray());
			var rootParam = Expression.Parameter(typeof(object));
			var cmdParam = Expression.Parameter(typeof(IDbCommand));

			Expression parameterAccessor = null;

			if (expQueue.Count > 1)
			{
				while (expQueue.Count != 0)
				{
					var exp = expQueue.Pop();
					if (exp == null) // static type
						throw new ObjectSqlException("Invalid constant accessor detected. You can not use static fields as constants holders. Consider passing this value as variable");
					if (exp.NodeType == ExpressionType.Constant)
						parameterAccessor = Expression.Convert(rootParam, exp.Type);
					else if (exp.NodeType == ExpressionType.MemberAccess)
						parameterAccessor = Expression.MakeMemberAccess(parameterAccessor, ((MemberExpression)exp).Member);
					else throw new ObjectSqlException("Invalid constant accessor detected");
				}
			}
			else if (expQueue.Peek().NodeType != ExpressionType.Constant)
				throw new ObjectSqlException("Invalid constant accessor detected");
			else if (((ConstantExpression)expQueue.Peek()).Value != null)
				parameterAccessor = expQueue.Pop();
			else
				parameterAccessor = Expression.Constant(DBNull.Value);

			var paramType = parameterAccessor.Type;

			if (!paramType.IsValueType)
				parameterAccessor = Expression.Condition(
					Expression.Equal(parameterAccessor, Expression.Constant(null)),
					Expression.Convert(Expression.Constant(DBNull.Value), typeof(object)),
					Expression.Convert(parameterAccessor, typeof(object)));
			else if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof (Nullable<>))
				parameterAccessor = Expression.Condition(
					Expression.MakeMemberAccess(parameterAccessor, paramType.GetProperty("HasValue")),
					Expression.Convert(Expression.MakeMemberAccess(parameterAccessor, paramType.GetProperty("Value")), typeof(object)),
					Expression.Convert(Expression.Constant(DBNull.Value), typeof(object)));
			else
				parameterAccessor = Expression.Convert(parameterAccessor, typeof(object));
			
			Expression parameterCreate = CreateParameterFactory(parameterName, parameterAccessor, parameterType);

			Expression parameterAdd = Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<IDbCommand>(c => c.Parameters));
			parameterAdd = Expression.Call(parameterAdd, Reflect.FindMethod<IDbCommand>(c => c.Parameters.Add(default(object))), parameterCreate);

			return Expression.Lambda<Action<IDbCommand, object>>(
						parameterAdd,
						cmdParam,
						rootParam).Compile();
		}
		public Action<IDbCommand, object> CreateInsertionParametersInitializerAction(EntitySchema entitySchema, EntityInsertionInformation insertionInfo)
		{
			var sbAppend = Reflect.FindMethod<StringBuilder>(s => s.Append(""));
			// input cmd
			var dbCmdParam = Expression.Parameter(typeof(IDbCommand), "dbCommand");
			// input object
			var objParam = Expression.Parameter(typeof(object), "param");
			// Entity[] e;
			var ent = Expression.Variable(entitySchema.EntityType.MakeArrayType(), "entities");
			//StringBuilder sb;
			var sb = Expression.Variable(typeof(StringBuilder), "sb");

			var index = Expression.Variable(typeof(int), "rowIndex");
			var dbParamIndex = Expression.Variable(typeof(int), "dbParamIndex");
			var dbParamName = Expression.Variable(typeof(string), "dbParamName");
			var breakLable = Expression.Label();

			var methodBody = new List<Expression>();
			//sb = inputObj as Entity[]
			methodBody.Add(Expression.Assign(ent, Expression.Convert(objParam, ent.Type)));
			//sb = new StringBuilder();
			methodBody.Add(Expression.Assign(sb, Expression.New(Reflect.FindCtor(() => new StringBuilder()))));
			//sb.Append(dbCommand.CommandText);
			methodBody.Add(Expression.Call(sb, sbAppend, Expression.MakeMemberAccess(dbCmdParam, Reflect.FindProperty<IDbCommand>(c => c.CommandText))));
			//sb.Append(" VALUES (")
			methodBody.Add(Expression.Call(sb, sbAppend, Expression.Constant(" VALUES ")));
			// while(true)
			var loopBody = new List<Expression>();
			//if (index == ent.Length)
			loopBody.Add(Expression.IfThen(Expression.GreaterThanOrEqual(index, Expression.ArrayLength(ent)),
				//break;
				Expression.Break(breakLable)));
			//if (index>0)
			loopBody.Add(Expression.IfThen(Expression.GreaterThan(index, Expression.Constant(0)),
				// sb.Append(",")
				Expression.Call(sb, sbAppend, Expression.Constant(", "))));
			loopBody.Add(Expression.Call(sb, sbAppend, Expression.Constant("(")));
			for (int i = 0; i < insertionInfo.PropertiesIndexesToInsert.Length; i++)
			{
				var prop = entitySchema.GetEntityPropertyByIndex(insertionInfo.PropertiesIndexesToInsert[i]);
				var propAccess = Expression.MakeMemberAccess(Expression.ArrayIndex(ent, index), prop.PropertyInfo);
				// sb.Append(", ");
				if (i > 0)
					loopBody.Add(Expression.Call(sb, sbAppend, Expression.Constant(", ")));
				#region dbParamName = "p"+index; sb.Append("@"+dbParamName); cmd.Parameters.Add(new param);
				var setupParam = new List<Expression>();

				//dbParamName = "p"+dbParamIndex;
				setupParam.Add(Expression.Assign(dbParamName,
					Expression.Call(Reflect.FindMethod(() => String.Format("", default(object))),
						Expression.Constant("p{0}"), Expression.Convert(dbParamIndex, typeof(object)))));

				// setup string
				setupParam.Add(Expression.Call(sb, sbAppend, Expression.Constant("@")));
				setupParam.Add(Expression.Call(sb, sbAppend, dbParamName));
				setupParam.Add(Expression.Assign(dbParamIndex, Expression.Increment(dbParamIndex)));
				// add parameter
				var parameterAccessor = Expression.Convert(propAccess, typeof(object));
				var parameterCreate = CreateParameterFactory(dbParamName, parameterAccessor, prop.StorageField.DbType);
				Expression parameterAdd = Expression.MakeMemberAccess(dbCmdParam, Reflect.FindProperty<IDbCommand>(c => c.Parameters));
				parameterAdd = Expression.Call(parameterAdd, Reflect.FindMethod<IDbCommand>(c => c.Parameters.Add(default(object))), parameterCreate);
				setupParam.Add(parameterAdd);
				#endregion
				if (prop.PropertyInfo.PropertyType.IsValueType &&
					(!prop.PropertyInfo.PropertyType.IsGenericType ||
						prop.PropertyInfo.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>)))
				{
					loopBody.AddRange(setupParam);
				}
				else
				{
					//if (prop == null)
					loopBody.Add(Expression.IfThenElse(Expression.Equal(propAccess, Expression.Constant(null, prop.PropertyInfo.PropertyType)),
						// s.Append(NULL)
									Expression.Call(sb, sbAppend, Expression.Constant("NULL")),
									Expression.Block(setupParam)));
				}
			}
			loopBody.Add(Expression.Call(sb, sbAppend, Expression.Constant(")")));
			//index ++
			loopBody.Add(Expression.Assign(index, Expression.Increment(index)));

			methodBody.Add(Expression.Loop(Expression.Block(loopBody.ToArray())));
			methodBody.Add(Expression.Label(breakLable));
			//cmd.CommandText+=""
			methodBody.Add(Expression.Assign(
				Expression.MakeMemberAccess(dbCmdParam, Reflect.FindProperty<IDbCommand>(c => c.CommandText)),
				Expression.Call(sb, Reflect.FindMethod<StringBuilder>(s => s.ToString()))));

			return Expression.Lambda<Action<IDbCommand, object>>(
						Expression.Block(new[] { ent, sb, index, dbParamIndex, dbParamName }, methodBody.ToArray()),
						dbCmdParam, objParam).Compile();
		}

		#region materialization
		public Delegate CreateEntityMaterializationDelegate(EntitySchema schema, EntityMaterializationInformation materializationInfo)
		{
			Type delegateType = typeof(Func<,>).MakeGenericType(typeof(IDataReader), schema.EntityType);

			if (materializationInfo.IsSingleValue && !materializationInfo.UseResultMapping)
				return CreateSingleValueRowFactory(schema, delegateType);
			
			if (materializationInfo.IsConstructorBased)
				return CreateConstructorBasedRowFactory(materializationInfo.ConstructorInfo, delegateType);

			if (materializationInfo.IsSingleValue && materializationInfo.UseResultMapping)
				return CreateResultMappingFactory(schema, delegateType);

			return CreatePropertyInitializationBasedRowFactory(schema, materializationInfo.FieldsIndexes, delegateType);
		}
		
		private Delegate CreateSingleValueRowFactory(EntitySchema schema, Type delegateType)
		{
			var dataReaderParameter = Expression.Parameter(typeof(IDataReader));
			var reader = GenerateReadValueFromDatabase(dataReaderParameter, schema.EntityType, 0);

			return Expression.Lambda(delegateType, reader, dataReaderParameter).Compile();
		}
		private static Delegate CreatePropertyInitializationBasedRowFactory(EntitySchema entitySchema, int[] fieldIndexes, Type delegateType)
		{
			var dataReaderParameter = Expression.Parameter(typeof(IDataReader));

			var bindings = new List<MemberBinding>();
			int dbReaderIndex = 0;
			foreach (var fldIndex in fieldIndexes)
			{
				var property = entitySchema.GetEntityPropertyByIndex(fldIndex).PropertyInfo;
				bindings.Add(Expression.Bind(property,
						GenerateReadValueFromDatabase(dataReaderParameter, property.PropertyType, dbReaderIndex)));
				++dbReaderIndex;
			}

			return Expression.Lambda(
				delegateType,
				Expression.MemberInit(
					Expression.New(entitySchema.EntityType.GetConstructor(new Type[0])), bindings.ToArray()),
					dataReaderParameter)
				.Compile();
		}
		private static Delegate CreateConstructorBasedRowFactory(ConstructorInfo ctorInfo, Type delegateType)
		{
			var dataReaderParameter = Expression.Parameter(typeof(IDataReader));

			var bindings = new List<Expression>();

			var parameters = ctorInfo.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				bindings.Add(GenerateReadValueFromDatabase(dataReaderParameter, parameters[i].ParameterType, i));
			}

			return Expression.Lambda(
				delegateType,
				Expression.New(ctorInfo, bindings.ToArray()),
					dataReaderParameter)
				.Compile();
		}
		/*IDataReader dr;
			dynamic res = new object();
			for (int i = 0; i < dr.FieldCount; i++)
			{
				var fieldName = dr.GetName(i);
				var fieldType = dr.GetFieldType(i);
				if (fieldName.Equals("myField", StringComparison.CurrentCultureIgnoreCase) && fieldType == typeof (string))
					res.myField = (string) dr.GetValue(i);
				else (fieldName.Equals("myField2", StringComparison.CurrentCultureIgnoreCase) && fieldType == typeof (string))
					res.myField2 = (string) dr.GetValue(i);
			}*/
		private Delegate CreateResultMappingFactory(EntitySchema schema, Type delegateType)
		{
			var parts = new List<Expression>();
			var fieldCountProp = (PropertyInfo)Reflect.FindProperty<IDataReader>(r => r.FieldCount);
			var getName = Reflect.FindMethod<IDataReader>(r => r.GetName(0));
			var getType = Reflect.FindMethod<IDataReader>(r => r.GetFieldType(0));
			var getValue = Reflect.FindMethod<IDataReader>(r => r.GetValue(0));

			var stringEquals = Reflect.FindMethod<string>(r => r.Equals("", StringComparison.OrdinalIgnoreCase));

			var exitLabel = Expression.Label();
			var continueLabel = Expression.Label();

			var dataReaderParameter = Expression.Parameter(typeof(IDataReader));
			
			var index = Expression.Variable(typeof (int), "index");
			var assignIndex = Expression.Assign(index, Expression.Constant(0));
			parts.Add(assignIndex);

			var newRow = Expression.Variable(schema.EntityType, "newRow");
			var assignNewRow = Expression.Assign(newRow, Expression.New(schema.EntityType.GetConstructor(new Type[0])));
			parts.Add(assignNewRow);

			parts.Add(Expression.Label(continueLabel));
			// Loop ---------
			var loopExitCondition = Expression.IfThen(
							Expression.Equal(index, Expression.Property(dataReaderParameter, fieldCountProp)),
							Expression.Goto(exitLabel));
			parts.Add(loopExitCondition);

			var fieldName = Expression.Variable(typeof (string), "fieldName");
			var fieldNameAssign = Expression.Assign(fieldName, Expression.Call(dataReaderParameter, getName, index));
			parts.Add(fieldNameAssign);

			var fieldType = Expression.Variable(typeof (Type), "fieldType");
			var fieldTypeAssign = Expression.Assign(fieldType, Expression.Call(dataReaderParameter, getType, index));
			parts.Add(fieldTypeAssign);
				// ifs ---
			foreach (var p in schema.EntityFields)
			{
				var ifExp = Expression.IfThen(
									Expression.Call(fieldName, stringEquals, Expression.Constant(p.StorageField.Name),Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
									Expression.Assign(Expression.Property(newRow, p.PropertyInfo),
												Expression.Convert(Expression.Call(dataReaderParameter,getValue,index), p.PropertyInfo.PropertyType)));
				parts.Add(ifExp);
			}
				// -------
			parts.Add(Expression.Assign(index, Expression.Add(index, Expression.Constant(1))));
			parts.Add(Expression.Goto(continueLabel));

			//------------
			parts.Add(Expression.Label(exitLabel));
			parts.Add(newRow);

			var body = Expression.Block(
				new[]{index, newRow,fieldName,fieldType},
				parts);

			return Expression.Lambda(delegateType, body, dataReaderParameter).Compile();
		}

		private static Expression GenerateReadValueFromDatabase(ParameterExpression parameter, Type fieldType, int propertyIndex)
		{
			var methodInfo = FindReaderMethod(fieldType);
			var readMethod = methodInfo != null
								? Expression.Call(parameter, methodInfo, Expression.Constant(propertyIndex, typeof(int)))
								: (Expression)Expression.Convert(
									Expression.Call(parameter, Reflect.FindMethod<IDataReader>(r => r.GetValue(1)), LambdaExpression.Constant(propertyIndex, typeof(int))),
									fieldType);
			if (!fieldType.IsValueType ||
				(fieldType.IsGenericType &&
				 fieldType.GetGenericTypeDefinition() == typeof(Nullable<>)))
			{
				readMethod =
					Expression.Condition(
						Expression.Call(parameter, Reflect.FindMethod<IDataReader>(dr => dr.IsDBNull(1)), LambdaExpression.Constant(propertyIndex, typeof(int))),
						Expression.Constant(null, fieldType),
						Expression.Convert(readMethod, fieldType));
			}
			return readMethod;
		}

		private static MethodInfo FindReaderMethod(Type type)
		{
			if (type == typeof(int) || type == typeof(int?))
				return Reflect.FindMethod<IDataReader>(r => r.GetInt32(1));
			if (type == typeof(short) || type == typeof(short?))
				return Reflect.FindMethod<IDataReader>(r => r.GetInt16(1));
			if (type == typeof(long) || type == typeof(long?))
				return Reflect.FindMethod<IDataReader>(r => r.GetInt64(1));
			if (type == typeof(string))
				return Reflect.FindMethod<IDataReader>(r => r.GetString(1));
			if (type == typeof(bool) || type == typeof(bool?))
				return Reflect.FindMethod<IDataReader>(r => r.GetBoolean(1));
			if (type == typeof(byte) || type == typeof(byte?))
				return Reflect.FindMethod<IDataReader>(r => r.GetByte(1));
			if (type == typeof(DateTime) || type == typeof(DateTime?))
				return Reflect.FindMethod<IDataReader>(r => r.GetDateTime(1));
			if (type == typeof(decimal) || type == typeof(decimal?))
				return Reflect.FindMethod<IDataReader>(r => r.GetDecimal(1));
			if (type == typeof(double) || type == typeof(double?))
				return Reflect.FindMethod<IDataReader>(r => r.GetDouble(1));
			if (type == typeof(float) || type == typeof(float?))
				return Reflect.FindMethod<IDataReader>(r => r.GetFloat(1));
			if (type == typeof(Guid) || type == typeof(Guid?))
				return Reflect.FindMethod<IDataReader>(r => r.GetGuid(1));

			return null;
		}
		#endregion

		protected abstract Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor, IStorageFieldType storageParameterType);


		public Action<IDbCommand, object> CreateChangeDatabaseCommandTypeAction(CommandType commandType)
		{
			var cmdParam = Expression.Parameter(typeof(IDbCommand));
			var rootParam = Expression.Parameter(typeof(object));

			return Expression.Lambda<Action<IDbCommand, object>>(
						Expression.Assign(
							Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<IDbCommand>(c => c.CommandType)),
							Expression.Constant(commandType)),
						cmdParam,
						rootParam).Compile();
		}
		
	}
}
