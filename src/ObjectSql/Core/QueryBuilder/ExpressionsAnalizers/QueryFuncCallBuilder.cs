using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.SchemaManager.EntitySchema;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class QueryFuncCallBuilder : ExpressionVisitor, ISqlQueryBuilder
	{
		private BuilderContext _context;
		protected IEntitySchemaManager SchemaManager { get; private set; }
		protected ICommandPreparatorsHolder CommandPreparatorsHolder { get { return _context.Preparators; } }
		protected IDelegatesBuilder ExpressionBuilder { get; private set; }
		private ParameterExpression[] _parameters;
		public QueryFuncCallBuilder(IEntitySchemaManager schemaManager, IDelegatesBuilder expressionBuilder)
		{
			SchemaManager = schemaManager;
			ExpressionBuilder = expressionBuilder;
		}

		public IStorageFieldType DbTypeInContext { get; set; }

		public string BuildSql(BuilderContext context, ParameterExpression[] parameters, Expression expression)
		{
			_context = context;
			_parameters = parameters;
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
				var initializer = CreateParameterInitializer(_context.Context.SqlPart.QueryRoots, funcParam.Name, valueAccessor, funcParam.DbType, funcParam.Direction);

				var descriptor = new CommandParameterPreProcessor(funcParam.Name, funcParam.DbType, node.Arguments[i], initializer);

				CommandPreparatorsHolder.AddPreProcessor(descriptor);

				if (funcParam.IsOut)
				{
					var parameterReader = CreateParameterReader(_context.Context.SqlPart.QueryRoots, funcParam.Name, valueAccessor);
					var postProcessor = new StoredProcedureOutParameterProcessor(parameterReader);
					CommandPreparatorsHolder.AddPostProcessor(postProcessor);
				}

			}

			return node;
		}

		private Action<IDbCommand, QueryRoots> CreateParameterReader(QueryRoots queryRoots, string name, Expression accessor)
		{
			return ExpressionBuilder.CreateCommandParameterReader(queryRoots, Expression.Constant(name, typeof(string)), accessor);
		}

		protected Action<IDbCommand, QueryRoots> CreateParameterInitializer(QueryRoots roots, string name, Expression accessor, IStorageFieldType dbTypeInContext, ParameterDirection direction)
		{
			return ExpressionBuilder.CreateDatabaseParameterFactoryAction(roots, Expression.Constant(name, typeof(string)), accessor, dbTypeInContext, direction);
		}

		private static bool IsNullConstant(Expression accessor)
		{
			var expressions = ExpressionEnumerator.Enumerate(accessor).ToArray();
			var isConstant = (expressions.Length == 1 && (expressions[0] is ConstantExpression) &&
				((ConstantExpression)expressions[0]).Value == null);
			return isConstant;
		}


		public string BuildSql(IStorageFieldType dbTypeInContext, Expression expression)
		{
			throw new NotImplementedException();
		}


		public string BuildSql(Expression expression)
		{
			throw new NotImplementedException();
		}
	}
}
