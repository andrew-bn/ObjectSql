using System.Collections.Generic;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Core.SchemaManager.EntitySchema;
using ObjectSql.Exceptions;
using ObjectSql.QueryImplementation;
using ObjectSql.QueryInterfaces;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class QueryExpressionBuilder : ExpressionVisitor, ISqlQueryBuilder
	{
		protected BuilderContext BuilderContext { get; private set; }
		protected IEntitySchemaManager SchemaManager { get; private set; }
		protected ICommandPreparatorsHolder CommandPreparatorsHolder { get { return BuilderContext.Preparators; } }
		protected IDelegatesBuilder DelegatesBuilder { get; private set; }
		protected SqlWriter SqlWriter { get; private set; }
		protected CommandText Text { get; set; }
		protected ParameterExpression[] ExpressionParameters { get; set; }
		public QueryExpressionBuilder(IEntitySchemaManager schemaManager,
			IDelegatesBuilder expressionBuilder, SqlWriter sqlWriter)
		{
			SchemaManager = schemaManager;
			DelegatesBuilder = expressionBuilder;
			SqlWriter = sqlWriter;
		}
		public virtual string BuildSql(BuilderContext context, ParameterExpression[] parameters, Expression expression)
		{
			BuilderContext = context;
			ExpressionParameters = parameters;
			return BuildSql(expression);
		}
		public string BuildSql(Expression expression)
		{
			var buf = Text;
			Text = new CommandText();
			Visit(expression);
			var result = Text.ToString();
			Text = buf;
			return result;
		}
		private void AddParameter(Expression accessor)
		{
			if (accessor.NodeType == ExpressionType.Constant &&
				((ConstantExpression)accessor).Value == null)
			{
				SqlWriter.WriteNull(Text);
			}
			else
			{

				var param = GetParameterDescriptor(accessor);
				SqlWriter.WriteParameter(Text, param.Name);
			}
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (!node.ContainsSql())
				AddParameter(node);
			else
				SqlWriter.WriteExpression(this, BuilderContext, Text, node);

			return node;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			SqlWriter.WriteName(BuilderContext, Text, "", node.Name);
			return node;
		}
		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			var buf = Text;

			var parameters = new List<string>();

			foreach (var item in node.Expressions)
			{
				Text = new CommandText();
				Visit(item);
				parameters.Add(Text.ToString());
			}

			Text = buf;
			Text.Append(string.Join(", ", parameters));

			return node;
		}
		protected override Expression VisitMember(MemberExpression node)
		{
			if (!node.ContainsSql())
			{
				AddParameter(node);
			}
			else if (node.Expression.NodeType == ExpressionType.Parameter)
			{
				var entityType = node.Member.DeclaringType;
				var aliasName = ((ParameterExpression)node.Expression).Name;
				var fieldName = node.Member.Name;

				WriteStorageFieldAccess(entityType, aliasName, fieldName);
			}
			else if (node.Expression is MemberExpression &&
				((MemberExpression)node.Expression).Expression.Type.IsGenericType &&
				((MemberExpression)node.Expression).Expression.Type.GetGenericTypeDefinition() == typeof(ParametersSubstitutor<>))
			{

				var entityType = ((MemberExpression)node.Expression).Expression.Type.GetGenericArguments()[0];
				var aliasName = ((IParameterSubstitutor)((ConstantExpression)((MemberExpression)node.Expression).Expression).Value).Name;
				var fieldName = node.Member.Name;

				WriteStorageFieldAccess(entityType, aliasName, fieldName);
			}
			else
			{
				SqlWriter.WriteExpression(this, BuilderContext, Text, node);
			}

			return node;
		}

		private void WriteStorageFieldAccess(Type entityType, string aliasName, string fieldName)
		{
			var entitySchema = SchemaManager.GetSchema(entityType);
			var storageField = entitySchema.GetStorageField(fieldName);
			BuilderContext.DbTypeInContext = storageField.DbType;
			SqlWriter.WriteName(BuilderContext, Text, aliasName, storageField.Name);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node.ContainsSql())
				SqlWriter.WriteExpression(this, BuilderContext, Text, node);
			else
				AddParameter(node);

			return node;
		}
		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node.ContainsSql())
				SqlWriter.WriteExpression(this, BuilderContext, Text, node);
			else
				AddParameter(node);

			return node;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (!node.ContainsSql())
			{
				AddParameter(node);
			}
			else if (!typeof(IQuery).IsAssignableFrom(node.Type))//not nested query
			{
				SqlWriter.WriteExpression(this, BuilderContext, Text, node);
			}
			else//is nested query
			{
				MemberExpression sqlQueryNode = null;
				node.Visit<MemberExpression>((v, e) => { if (sqlQueryNode == null && e.Member == typeof(Sql).GetProperty("Query")) sqlQueryNode = e; return e; });
				var param = Expression.Parameter(typeof(Query));
				var newNode = node.Visit<MemberExpression>((v, e) => (e == sqlQueryNode) ? param : (Expression)e);
				newNode = newNode.Visit<ParameterExpression>((v, e) => SubstituteParameter(ExpressionParameters, e));
				var queryBuilder = Expression.Lambda<Func<Query, IQueryEnd>>(newNode, param).Compile();

				var ctx = new QueryContext(BuilderContext.Context.InitialConnectionString,
										   BuilderContext.Context.Command, BuilderContext.Context.ResourcesTreatmentType,
										   BuilderContext.Context.QueryEnvironment);

				var query = new Query(ctx);
				queryBuilder(query);

				foreach (var root in BuilderContext.Context.SqlPart.QueryRoots.Roots)
					query.Context.SqlPart.QueryRoots.AddRoot(root);

				query.Context.SqlPart.BuilderContext.Preparators = BuilderContext.Preparators;

				query.Context.SqlPart.BuildPart();
				Text.Append(query.Context.SqlPart.BuilderContext.Text.ToString());
			}

			return node;
		}

		private Expression SubstituteParameter(IEnumerable<ParameterExpression> parameters, ParameterExpression node)
		{
			if (parameters.Contains(node))
			{
				var s = (IParameterSubstitutor)Activator.CreateInstance(typeof(ParametersSubstitutor<>).MakeGenericType(node.Type));
				s.Name = node.Name;
				return Expression.MakeMemberAccess(Expression.Constant(s, s.GetType()), s.GetType().GetProperty("Table"));
			}
			return node;
		}


		private CommandParameterPreProcessor GetParameterDescriptor(Expression accessor)
		{
			var dbTypeInContext = BuilderContext.DbTypeInContext;
			var descriptor = CommandPreparatorsHolder.PreProcessors
											 .Select(p => p as CommandParameterPreProcessor)
											 .Where(p => p != null)
											 .SingleOrDefault(d => ExpressionComparer.AreEqual(d.ValueAccessorExp, accessor) && Equals(d.DbType, dbTypeInContext));

			if (descriptor == null)
			{
				var parameterName = "p" + CommandPreparatorsHolder.PreProcessors.Count(p => p is CommandParameterPreProcessor);

				if (accessor.Type.IsArray)
					parameterName += "_" + Guid.NewGuid().ToString().Replace("-", "");

				var initializer = accessor.Type.IsArray
									? CreateArrayParameterInitializer(parameterName, accessor, dbTypeInContext)
									: CreateParameterInitializer(parameterName, accessor, dbTypeInContext);

				descriptor = new CommandParameterPreProcessor(parameterName, dbTypeInContext, accessor, initializer);
				CommandPreparatorsHolder.AddPreProcessor(descriptor);
			}

			descriptor.RootIndex = accessor.IndexOfRoot(BuilderContext.Context.SqlPart.QueryRoots);

			return descriptor;
		}

		protected Action<IDbCommand, object> CreateParameterInitializer(string name, Expression accessor, IStorageFieldType dbTypeInContext)
		{
			return DelegatesBuilder.CreateDatabaseParameterFactoryAction(Expression.Constant(name, typeof(string)), accessor, dbTypeInContext, ParameterDirection.Input);
		}
		protected Action<IDbCommand, object> CreateArrayParameterInitializer(string name, Expression accessor, IStorageFieldType dbTypeInContext)
		{
			return DelegatesBuilder.CreateArrayParameters(name, accessor, dbTypeInContext, ParameterDirection.Input);
		}
	}
}
