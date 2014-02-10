using SqlBoost.Core.Bo.CommandPreparatorDescriptor;
using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Core.Misc;
using SqlBoost.Core.QueryBuilder.LambdaBuilder;
using SqlBoost.Core.SchemaManager;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace SqlBoost.Core.QueryBuilder.ExpressionsAnalizers
{
	internal class QueryFuncCallBuilder : ExpressionVisitor, ISqlQueryBuilder
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
				var funcParam = funcSchema.FuncParameters.First(p => p.Index == i).StorageField;
				var initializer = CreateParameterInitializer(funcParam.Name, node.Arguments[i], funcParam.DbType);
				
				var descriptor = IsConstant(node.Arguments[i])
									? new DatabaseCommandConstantPreparator(funcParam.Name, funcParam.DbType, node.Arguments[i], initializer)
									: (SingleParameterPreparator) new DatabaseCommandParameterPreparator(funcParam.Name, funcParam.DbType, node.Arguments[i], initializer);

				CommandPreparatorsHolder.AddPreparator(descriptor);

				if (descriptor.RootDemanding)
				{
					descriptor.AsDatabaseParameter().ParameterWasEncountered(CommandPreparatorsHolder.ParametersEncountered);
					CommandPreparatorsHolder.ParametersEncountered++;
				}
			}

			return node;
		}

		protected Action<IDbCommand, object> CreateParameterInitializer(string name, Expression accessor, IStorageFieldType dbTypeInContext)
		{
			return ExpressionBuilder.CreateDatabaseParameterFactoryAction(Expression.Constant(name, typeof(string)), accessor, dbTypeInContext);
		}

		private static bool IsConstant(Expression accessor)
		{
			var expressions = ExpressionEnumerator.Enumerate(accessor).ToArray();
			var isConstant = expressions.Length == 1 && (expressions[0] is ConstantExpression);
			return isConstant;
		}
	}
}
