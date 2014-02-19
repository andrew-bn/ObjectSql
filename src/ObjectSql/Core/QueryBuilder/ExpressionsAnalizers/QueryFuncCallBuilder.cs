﻿using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class QueryFuncCallBuilder : ExpressionVisitor, ISqlQueryBuilder
	{
		protected IEntitySchemaManager SchemaManager { get; private set; }
		protected ICommandPreparatorsHolder CommandPreparatorsHolder { get; private set; }
		protected IDelegatesBuilder ExpressionBuilder { get; private set; }
		public QueryFuncCallBuilder(IEntitySchemaManager schemaManager, IDelegatesBuilder expressionBuilder)
		{
			SchemaManager = schemaManager;
			ExpressionBuilder = expressionBuilder;
		}
		public string BuildSql(ICommandPreparatorsHolder commandPreparators, Expression expression, bool useAliases)
		{
			CommandPreparatorsHolder = commandPreparators;
			Visit(expression);
			return null;
		}
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var funcSchema = SchemaManager.GetFuncSchema(node.Method);

			for (int i = 0; i < node.Arguments.Count; i++)
			{
				var valueAccessor = node.Arguments[i].StripConvert();
				var funcParam = funcSchema.FuncParameters.First(p => p.Index == i).StorageParameter;
				var initializer = CreateParameterInitializer(funcParam.Name, valueAccessor, funcParam.DbType,funcParam.Direction);

				var descriptor = IsConstant(valueAccessor)
									? new DatabaseCommandConstantPrePostProcessor(funcParam.Name, funcParam.DbType, valueAccessor, initializer)
									: (SingleParameterPrePostProcessor)new DatabaseCommandParameterPrePostProcessor(funcParam.Name, funcParam.DbType, node.Arguments[i], initializer);

				CommandPreparatorsHolder.AddPreProcessor(descriptor);

				if (descriptor.RootDemanding)
				{
					descriptor.AsDatabaseParameter().ParameterWasEncountered(CommandPreparatorsHolder.ParametersEncountered);

					if (funcParam.IsOut && !IsConstant(valueAccessor))
					{
						var parameterReader = CreateParameterReader(funcParam.Name, valueAccessor);
						var postProcessor = new StoredProcedureOutParameterProcessor(parameterReader);
						CommandPreparatorsHolder.AddPostProcessor(postProcessor);
						postProcessor.AsStoredProcedureOutParameterProcessor().ParameterWasEncountered(CommandPreparatorsHolder.ParametersEncountered);
					}

					CommandPreparatorsHolder.ParametersEncountered++;
				}
			}

			return node;
		}

		private Action<IDbCommand, object> CreateParameterReader(string name, Expression accessor)
		{
			return ExpressionBuilder.CreateCommandParameterReader(Expression.Constant(name, typeof(string)), accessor);
		}

		protected Action<IDbCommand, object> CreateParameterInitializer(string name, Expression accessor, IStorageFieldType dbTypeInContext, ParameterDirection direction)
		{
			return ExpressionBuilder.CreateDatabaseParameterFactoryAction(Expression.Constant(name, typeof(string)), accessor, dbTypeInContext,direction);
		}

		private static bool IsConstant(Expression accessor)
		{
			var expressions = ExpressionEnumerator.Enumerate(accessor).ToArray();
			var isConstant = (expressions.Length == 1 && (expressions[0] is ConstantExpression));
			return isConstant;
		}
	}
} 
