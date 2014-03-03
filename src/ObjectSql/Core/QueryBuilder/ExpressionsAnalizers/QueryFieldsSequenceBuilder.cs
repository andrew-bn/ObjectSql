using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Exceptions;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class QueryFieldsSequenceBuilder: QueryExpressionBuilder
	{
		public QueryFieldsSequenceBuilder(IEntitySchemaManager schemaManager, IDelegatesBuilder expressionBuilder, ISqlWriter sqlWriter)
			: base(schemaManager, expressionBuilder, sqlWriter)
		{
		}
		private bool _multiFieldSelectionFromTable = true;
		public override string BuildSql(BuilderContext context, Expression expression, bool useAliases)
		{
			_multiFieldSelectionFromTable = true;
			return base.BuildSql(context, expression, useAliases);
		}
		protected override Expression VisitParameter(ParameterExpression alias)
		{
			if (_multiFieldSelectionFromTable)
			{
				var entityFields = SchemaManager.GetSchema(alias.Type).EntityFields;

				var appendComma = false;
				foreach (var fld in entityFields)
				{
					if (appendComma) SqlWriter.WriteComma(Text);
					appendComma = true;
					SqlWriter.WriteName(Text, fld.StorageField.Name);
				}
			}
			else base.VisitParameter(alias);

			return alias;
		}
		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression.NodeType != ExpressionType.Parameter)
				throw new ObjectSqlException("Constants or complex entities are not allowed then you select fields sequence");
			
			return base.VisitMember(node);
		}
		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			throw new ObjectSqlException("Not anonimus types are not allowed then you select fields sequence");
		}
		protected override Expression VisitConstant(ConstantExpression node)
		{
			throw new ObjectSqlException("Constants are not allowed then you select fields sequence");
		}
		protected override Expression VisitNew(NewExpression node)
		{
			if (node.Members == null)
				throw new ObjectSqlException("Not anonimus types are not allowed then you select fields sequence");
			if (!_multiFieldSelectionFromTable)
				throw new ObjectSqlException("Complex types are not allowed");

			_multiFieldSelectionFromTable = false;
			
			for (int i = 0; i < node.Arguments.Count; i++)
			{
				if (node.Arguments[i].NodeType == ExpressionType.Parameter)
					throw new ObjectSqlException("Anonimus type with entity parameter encountered");

				if (i > 0) SqlWriter.WriteComma(Text);
				Visit(node.Arguments[i]);
			}
			return node;
		}
	}
}
