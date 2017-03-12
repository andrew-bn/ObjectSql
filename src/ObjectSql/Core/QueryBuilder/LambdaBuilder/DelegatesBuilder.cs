using System.Data.Common;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Misc;
using ObjectSql.Core.SchemaManager.EntitySchema;
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
		public Action<DbCommand, QueryRoots> CreateCommandParameterReader(QueryRoots roots,ConstantExpression parameterName, Expression valueAccessor)
		{
			var rootParam = Expression.Parameter(typeof(QueryRoots));
			var cmdParam = Expression.Parameter(typeof(DbCommand));
			var parameterAccessor = ReplaceConstantsToRootsAccessors(roots,valueAccessor,rootParam);

			Expression commandAccessor = Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<DbCommand>(c => c.Parameters));
			commandAccessor = Expression.MakeIndex(commandAccessor, typeof(DbParameterCollection).GetProperty("Item", typeof(DbParameter), new[] { typeof(string)}), new[] { parameterName });
			commandAccessor = Expression.Convert(commandAccessor, typeof (DbParameter));
			commandAccessor = Expression.MakeMemberAccess(commandAccessor, Reflect.FindProperty<DbParameter>(p => p.Value));

			commandAccessor = Expression.Condition(Expression.TypeIs(commandAccessor,typeof(DBNull)),
												   Expression.Convert(Expression.Constant(null), parameterAccessor.Type),
												   Expression.Convert(commandAccessor, parameterAccessor.Type));
	
			return Expression.Lambda<Action<DbCommand, QueryRoots>>(
						Expression.Assign(parameterAccessor, commandAccessor),
						cmdParam,
						rootParam).Compile();
		}

		public Action<DbCommand, QueryRoots> AddCommandReturnParameter(Type returnType, object dbType)
		{
			var parameterCreator = CreateCommandReturnParameter(returnType, dbType);

			var rootParam = Expression.Parameter(typeof(QueryRoots));
			var cmdParam = Expression.Parameter(typeof(DbCommand));
			Expression parameterAdd = Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<DbCommand>(c => c.Parameters));
			parameterAdd = Expression.Call(parameterAdd, Reflect.FindMethod<DbCommand>(c => c.Parameters.Add(default(object))), parameterCreator);

			return Expression.Lambda<Action<DbCommand, QueryRoots>>(
						parameterAdd,
						cmdParam,
						rootParam).Compile();
		}
		public Func<DbCommand, object> ReadCommandReturnParameter()
		{
			var cmdParam = Expression.Parameter(typeof (DbCommand));
			Expression result = Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<DbCommand>(c => c.Parameters));
			result = Expression.MakeIndex(result, typeof (DbParameterCollection).GetProperty("Item", typeof(DbParameter), new[] { typeof(string) }), new[]{Expression.Constant(ReturnParameterName)});
			result = Expression.MakeMemberAccess(Expression.Convert(result, typeof (DbParameter)), Reflect.FindProperty<DbParameter>(p => p.Value));
			result = Expression.Condition(Expression.TypeIs(result, typeof (DBNull)), Expression.Constant(null, typeof (object)), result);

			return Expression.Lambda<Func<DbCommand, object>>(result, cmdParam).Compile();
		}
		protected abstract string ReturnParameterName { get; }
		protected abstract Expression CreateCommandReturnParameter(Type returnType, object dbType);
		public Action<DbCommand, QueryRoots> CreateDatabaseParameterFactoryAction(QueryRoots roots, Expression parameterName, Expression valueAccessor, IStorageFieldType parameterType, ParameterDirection direction)
		{
			var rootParam = Expression.Parameter(typeof(QueryRoots));
			var cmdParam = Expression.Parameter(typeof(DbCommand));
			var parameterAccessor = ReplaceConstantsToRootsAccessors(roots, valueAccessor, rootParam);
		

			var paramType = parameterAccessor.Type;

			if (!paramType.IsValueType())
				parameterAccessor = Expression.Coalesce(parameterAccessor, Expression.Convert(Expression.Constant(DBNull.Value), typeof (object)));
			else if (paramType.IsGenericType() && paramType.GetGenericTypeDefinition() == typeof(Nullable<>))
				parameterAccessor = Expression.Coalesce(parameterAccessor, Expression.Convert(Expression.Constant(DBNull.Value), typeof (object)));
			else
				parameterAccessor = Expression.Convert(parameterAccessor, typeof(object));

			Expression parameterCreate = CreateParameterFactory(parameterName, parameterAccessor, parameterType,direction);

			Expression parameterAdd = Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<DbCommand>(c => c.Parameters));
			parameterAdd = Expression.Call(parameterAdd, Reflect.FindMethod<DbCommand>(c => c.Parameters.Add(default(object))), parameterCreate);

			return Expression.Lambda<Action<DbCommand, QueryRoots>>(
						parameterAdd,
						cmdParam,
						rootParam).Compile();
		}

		
		public Action<DbCommand, QueryRoots> CreateInsertionParametersInitializerAction(QueryRoots roots, EntitySchema entitySchema, EntityInsertionInformation insertionInfo)
		{
			var sbAppend = Reflect.FindMethod<StringBuilder>(s => s.Append(""));
			// input cmd
			var dbCmdParam = Expression.Parameter(typeof(DbCommand), "dbCommand");
			// input object
			var objParam = Expression.Parameter(typeof(QueryRoots), "param");
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
			methodBody.Add(Expression.Assign(ent, Expression.Convert(GetRootValueByIndex(objParam,roots.IndexOf(o=>o.GetType() == ent.Type)), ent.Type)));
			//sb = new StringBuilder();
			methodBody.Add(Expression.Assign(sb, Expression.New(Reflect.FindCtor(() => new StringBuilder()))));
			//sb.Append(dbCommand.CommandText);
			methodBody.Add(Expression.Call(sb, sbAppend, Expression.MakeMemberAccess(dbCmdParam, Reflect.FindProperty<DbCommand>(c => c.CommandText))));
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
				var parameterCreate = CreateParameterFactory(dbParamName, parameterAccessor, prop.StorageField.DbType, ParameterDirection.Input);
				Expression parameterAdd = Expression.MakeMemberAccess(dbCmdParam, Reflect.FindProperty<DbCommand>(c => c.Parameters));
				parameterAdd = Expression.Call(parameterAdd, Reflect.FindMethod<DbCommand>(c => c.Parameters.Add(default(object))), parameterCreate);
				setupParam.Add(parameterAdd);
				#endregion
				if (prop.PropertyInfo.PropertyType.IsValueType() &&
					(!prop.PropertyInfo.PropertyType.IsGenericType() ||
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
				Expression.MakeMemberAccess(dbCmdParam, Reflect.FindProperty<DbCommand>(c => c.CommandText)),
				Expression.Call(sb, Reflect.FindMethod<StringBuilder>(s => s.ToString()))));

			return Expression.Lambda<Action<DbCommand, QueryRoots>>(
						Expression.Block(new[] { ent, sb, index, dbParamIndex, dbParamName }, methodBody.ToArray()),
						dbCmdParam, objParam).Compile();
		}

		#region materialization
		public Delegate CreateEntityMaterializationDelegate(EntitySchema schema, EntityMaterializationInformation materializationInfo)
		{
			Type delegateType = typeof(Func<,>).MakeGenericType(typeof(DbDataReader), schema.EntityType);

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
			var dataReaderParameter = Expression.Parameter(typeof(DbDataReader));
			var reader = GenerateReadValueFromDatabase(dataReaderParameter, schema.EntityType, 0);

			return Expression.Lambda(delegateType, reader, dataReaderParameter).Compile();
		}
		private static Delegate CreatePropertyInitializationBasedRowFactory(EntitySchema entitySchema, int[] fieldIndexes, Type delegateType)
		{
			var dataReaderParameter = Expression.Parameter(typeof(DbDataReader));

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
			var dataReaderParameter = Expression.Parameter(typeof(DbDataReader));

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

		private Delegate CreateResultMappingFactory(EntitySchema schema, Type delegateType)
		{
			var parts = new List<Expression>();
			var fieldCountProp = (PropertyInfo)Reflect.FindProperty<DbDataReader>(r => r.FieldCount);
			var getName = Reflect.FindMethod<DbDataReader>(r => r.GetName(0));
			var getType = Reflect.FindMethod<DbDataReader>(r => r.GetFieldType(0));
			var getValue = Reflect.FindMethod<DbDataReader>(r => r.GetValue(0));

			var stringEquals = Reflect.FindMethod<string>(r => r.Equals("", StringComparison.OrdinalIgnoreCase));

			var exitLabel = Expression.Label();
			var continueLabel = Expression.Label();

			var dataReaderParameter = Expression.Parameter(typeof(DbDataReader));

			var value = Expression.Variable(typeof (object), "value");
			var index = Expression.Variable(typeof(int), "index");
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

			var fieldName = Expression.Variable(typeof(string), "fieldName");
			var fieldNameAssign = Expression.Assign(fieldName, Expression.Call(dataReaderParameter, getName, index));
			parts.Add(fieldNameAssign);

			var fieldType = Expression.Variable(typeof(Type), "fieldType");
			var fieldTypeAssign = Expression.Assign(fieldType, Expression.Call(dataReaderParameter, getType, index));
			parts.Add(fieldTypeAssign);
			
			Expression valueExp = Expression.Assign(value, Expression.Convert(Expression.Call(dataReaderParameter, getValue, index),typeof (object)));
			parts.Add(valueExp);

			// ifs ---
			foreach (var p in schema.EntityFields)
			{
				var propType = p.PropertyInfo.PropertyType;
				Expression readValue;

				Expression typeConverter = Expression.Convert(value, propType);

				if (propType.IsEnum()) 
				{
					typeConverter = Expression.Condition(Expression.TypeIs(value, typeof(string)),
													Expression.Convert(
														Expression.Call(typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) }),
																	Expression.Constant(propType),
																	Expression.Convert(value, typeof(string)),
																	Expression.Constant(true)),
														propType),
													Expression.Convert(value, propType));
				}
				if (propType.IsGenericType() && (propType.GetGenericTypeDefinition() == typeof (Nullable<>)) && 
								propType.GetGenericArguments()[0].IsEnum())
				{
					typeConverter = Expression.Condition(Expression.TypeIs(value, typeof(string)),
													Expression.Convert(
														Expression.Call(typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) }),
																	Expression.Constant(propType.GetGenericArguments()[0]),
																	Expression.Convert(value, typeof(string)),
																	Expression.Constant(true)),
														propType),
													Expression.Convert(value, propType));
				}

				if (propType.IsValueType() && (!propType.IsGenericType() || propType.GetGenericTypeDefinition() != typeof (Nullable<>)))
				{
					readValue = Expression.Condition(Expression.TypeIs(value, typeof(DBNull)),
													 Expression.Throw(Expression.Constant(new InvalidCastException("Unable to set null value to non nullable type for field '{0}'")),propType),
													 typeConverter);
				}
				else
				{
					readValue = Expression.Condition(Expression.TypeIs(value, typeof(DBNull)),
													 Expression.Constant(null, propType),
													 typeConverter);
				}

				readValue = Expression.TryCatch(readValue, Expression.Catch(typeof (InvalidCastException), Expression.Throw(
					Expression.Constant(new InvalidCastException(string.Format("Unable to cast result set value to Field '{0}'. Possible cast error of DBNull and non nullable type",p.PropertyInfo.Name))),readValue.Type)));

				var ifExp = Expression.IfThen(
					Expression.Call(fieldName, stringEquals, Expression.Constant(p.StorageField.Name), Expression.Constant(StringComparison.CurrentCultureIgnoreCase)),
					Expression.Assign(Expression.Property(newRow, p.PropertyInfo), readValue));

				parts.Add(ifExp);
			}
			// -------
			parts.Add(Expression.Assign(index, Expression.Add(index, Expression.Constant(1))));
			parts.Add(Expression.Goto(continueLabel));

			//------------
			parts.Add(Expression.Label(exitLabel));
			parts.Add(newRow);

			var body = Expression.Block(
				new[] { value, index, newRow, fieldName, fieldType },
				parts);

			return Expression.Lambda(delegateType, body, dataReaderParameter).Compile();
		}

		private static Expression GenerateReadValueFromDatabase(ParameterExpression parameter, Type fieldType, int propertyIndex)
		{
			var methodInfo = FindReaderMethod(fieldType);
			var readMethod = methodInfo != null
								? Expression.Call(parameter, methodInfo, Expression.Constant(propertyIndex, typeof(int)))
								: (Expression)Expression.Convert(
									Expression.Call(parameter, Reflect.FindMethod<DbDataReader>(r => r.GetValue(1)), LambdaExpression.Constant(propertyIndex, typeof(int))),
									fieldType);

			if (!fieldType.IsValueType() ||
				(fieldType.IsGenericType() &&
				 fieldType.GetGenericTypeDefinition() == typeof(Nullable<>)))
			{
				readMethod =
					Expression.Condition(
						Expression.Call(parameter, Reflect.FindMethod<DbDataReader>(dr => dr.IsDBNull(1)), LambdaExpression.Constant(propertyIndex, typeof(int))),
						Expression.Constant(null, fieldType),
						Expression.Convert(readMethod, fieldType));
			}

			return readMethod;
		}

		private static MethodInfo FindReaderMethod(Type type)
		{
			if (type == typeof(int) || type == typeof(int?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetInt32(1));
			if (type == typeof(short) || type == typeof(short?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetInt16(1));
			if (type == typeof(long) || type == typeof(long?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetInt64(1));
			if (type == typeof(string))
				return Reflect.FindMethod<DbDataReader>(r => r.GetString(1));
			if (type == typeof(bool) || type == typeof(bool?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetBoolean(1));
			if (type == typeof(byte) || type == typeof(byte?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetByte(1));
			if (type == typeof(DateTime) || type == typeof(DateTime?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetDateTime(1));
			if (type == typeof(decimal) || type == typeof(decimal?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetDecimal(1));
			if (type == typeof(double) || type == typeof(double?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetDouble(1));
			if (type == typeof(float) || type == typeof(float?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetFloat(1));
			if (type == typeof(Guid) || type == typeof(Guid?))
				return Reflect.FindMethod<DbDataReader>(r => r.GetGuid(1));

			return null;
		}
		#endregion

		protected abstract Expression CreateParameterFactory(Expression parameterName, Expression parameterAccessor, IStorageFieldType storageParameterType,ParameterDirection direction);


		public Action<DbCommand, QueryRoots> CreateChangeDatabaseCommandTypeAction(CommandType commandType)
		{
			var cmdParam = Expression.Parameter(typeof(DbCommand));
			var rootParam = Expression.Parameter(typeof(QueryRoots));

			return Expression.Lambda<Action<DbCommand, QueryRoots>>(
						Expression.Assign(
							Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<DbCommand>(c => c.CommandType)),
							Expression.Constant(commandType)),
						cmdParam,
						rootParam).Compile();
		}


		public Action<DbCommand, QueryRoots> CreateArrayParameters(QueryRoots roots, string paramName, Expression valueAccessor, IStorageFieldType parameterType, ParameterDirection direction)
		{
			var rootParam = Expression.Parameter(typeof(QueryRoots));
			var cmdParam = Expression.Parameter(typeof(DbCommand));

			Expression parameterName = Expression.Constant(paramName.Substring(0, paramName.IndexOf("_")));
			Expression parametersPlaceholder = Expression.Constant(paramName);
			var parameterAccessor = ReplaceConstantsToRootsAccessors(roots,valueAccessor,rootParam);


			var paramType = parameterAccessor.Type.GetElementType();


			var exprList = new List<Expression>();

			var indexVar = Expression.Variable(typeof (int), "index");
			var sb = Expression.Variable(typeof (StringBuilder), "sb");
			
			var loopLbl = Expression.Label("loopLabel");
			var exitLbl = Expression.Label("exitLabel");
			
			exprList.Add(Expression.Assign(sb,Expression.New(typeof(StringBuilder))));
			exprList.Add(Expression.Label(loopLbl));
			exprList.Add(Expression.IfThen(Expression.Equal(indexVar,
				Expression.MakeMemberAccess(parameterAccessor,Reflect.FindProperty<Array>(a=>a.Length))),
				Expression.Goto(exitLbl)));

			parameterAccessor = Expression.ArrayIndex(parameterAccessor, indexVar);

			if (!paramType.IsValueType())
				parameterAccessor = Expression.Condition(
					Expression.Equal(parameterAccessor, Expression.Constant(null)),
					Expression.Convert(Expression.Constant(DBNull.Value), typeof(object)),
					Expression.Convert(parameterAccessor, typeof(object)));
			else if (paramType.IsGenericType() && paramType.GetGenericTypeDefinition() == typeof(Nullable<>))
				parameterAccessor = Expression.Condition(
					Expression.MakeMemberAccess(parameterAccessor, paramType.GetProperty("HasValue")),
					Expression.Convert(Expression.MakeMemberAccess(parameterAccessor, paramType.GetProperty("Value")), typeof(object)),
					Expression.Convert(Expression.MakeMemberAccess(null, typeof(DBNull).GetField("Value")), typeof(object)));
			else
				parameterAccessor = Expression.Convert(parameterAccessor, typeof(object));

			var arrayParamName = Expression.Call(null, typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string), typeof(string) }), parameterName,
											Expression.Constant("_"), Expression.Call(indexVar, Reflect.FindMethod<int>(i => i.ToString())));


			var parameterCreate = CreateParameterFactory(arrayParamName, parameterAccessor, parameterType, direction);
			
			Expression parameterAdd = Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<DbCommand>(c => c.Parameters));
			parameterAdd = Expression.Call(parameterAdd, Reflect.FindMethod<DbCommand>(c => c.Parameters.Add(default(object))), parameterCreate);

			exprList.Add(Expression.IfThen(Expression.GreaterThan(indexVar,Expression.Constant(0)) , Expression.Call(sb, Reflect.FindMethod<StringBuilder>(s => s.Append("")), Expression.Constant(", "))));
			exprList.Add(Expression.Call(sb, Reflect.FindMethod<StringBuilder>(s => s.Append("")), Expression.Constant("@")));
			exprList.Add(Expression.Call(sb,Reflect.FindMethod<StringBuilder>(s=>s.Append("")),arrayParamName));
			exprList.Add(parameterAdd);
			exprList.Add(Expression.Assign(indexVar,Expression.Increment(indexVar)));
			exprList.Add(Expression.Goto(loopLbl));
			exprList.Add(Expression.Label(exitLbl));

			Expression changeCmd = Expression.MakeMemberAccess(cmdParam, Reflect.FindProperty<DbCommand>(c => c.CommandText));
			changeCmd = Expression.Assign(changeCmd,
			                              Expression.Call(changeCmd, Reflect.FindMethod<string>(s => s.Replace("", "")),
										  Expression.Call(null, typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }), Expression.Constant("@"), parametersPlaceholder),
										  Expression.Call(sb,Reflect.FindMethod<StringBuilder>(s=>s.ToString()))));

			exprList.Add(changeCmd);

			return Expression.Lambda<Action<DbCommand, QueryRoots>>(
						Expression.Block(new[]{indexVar,sb}, exprList),
						cmdParam,
						rootParam).Compile();
		}
		private static Expression ReplaceConstantsToRootsAccessors(QueryRoots roots, Expression valueAccessor, ParameterExpression rootParam)
		{
			return valueAccessor.Visit<ConstantExpression>((v, e) =>
			{
				if (roots.ContainsRoot(e.Value))
				{
					int rootIndex = roots.IndexOf(e.Value);
					return
						Expression.Convert(GetRootValueByIndex(rootParam, rootIndex),
						                   e.Type);
				}
				return e;
			});
		}

		private static MethodCallExpression GetRootValueByIndex(ParameterExpression rootParam, int rootIndex)
		{
			return Expression.Call(rootParam, Reflect.FindMethod<QueryRoots>(r => r.Get(0)),Expression.Constant(rootIndex));
		}
	}
}
