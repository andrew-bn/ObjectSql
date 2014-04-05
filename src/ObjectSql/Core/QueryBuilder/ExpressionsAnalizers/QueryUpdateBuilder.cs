
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Exceptions;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class QueryUpdateBuilder: QueryExpressionBuilder
	{
		private bool _initNodeEncountered;
		public QueryUpdateBuilder(IEntitySchemaManager schemaManager, IDelegatesBuilder expressionBuilder, SqlWriter sqlWriter)
			: base(schemaManager, expressionBuilder, sqlWriter)
		{
		}
		public override string BuildSql(BuilderContext context, ParameterExpression[] parameters, Expression expression)
		{
			_initNodeEncountered = false;
			var result = base.BuildSql(context,parameters, expression);
			if (!_initNodeEncountered)
				throw new ObjectSqlException("Update builder expects entity initialization");
			return result;
		}
		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			_initNodeEncountered = true;
			var entitySchema = SchemaManager.GetSchema(node.Type);
			for (int i = 0; i < node.Bindings.Count; i++)
			{
				if (i > 0) SqlWriter.WriteComma(Text);

				var storageField = entitySchema.GetStorageField(node.Bindings[i].Member.Name);
				SqlWriter.WriteName(BuilderContext, Text, "", storageField.Name);
				SqlWriter.WriteSet(Text);
				DbTypeInContext = storageField.DbType;
				Visit(((MemberAssignment)node.Bindings[i]).Expression);
			}
			Text.Append(" ");
			DbTypeInContext = null;
			return node;
		}
	}
}
