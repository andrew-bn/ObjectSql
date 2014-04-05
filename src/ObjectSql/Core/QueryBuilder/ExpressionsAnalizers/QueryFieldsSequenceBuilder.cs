using System.Linq;
using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Exceptions;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class QueryFieldsSequenceBuilder: QueryExpressionBuilder
	{
		public QueryFieldsSequenceBuilder(IEntitySchemaManager schemaManager, IDelegatesBuilder expressionBuilder, SqlWriter sqlWriter)
			: base(schemaManager, expressionBuilder, sqlWriter)
		{
		}
		private bool _multiFieldSelectionFromTable = true;
		public override string BuildSql(BuilderContext context, ParameterExpression[] parameters, Expression expression)
		{
			_multiFieldSelectionFromTable = true;
			return base.BuildSql(context,parameters, expression);
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
					SqlWriter.WriteName(BuilderContext, Text,"", fld.StorageField.Name);
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
			var props = SchemaManager.GetSchema(node.Type).EntityProperties;
			for (int i = 0; i < node.Bindings.Count; i++)
			{
				if (i > 0) SqlWriter.WriteComma(Text);

				var sf = props.FirstOrDefault(p=>p.Name == node.Bindings[i].Member.Name);
				if (sf.StorageField == null)
					SqlWriter.WriteName(BuilderContext, Text, "", sf.Name);
				else
					SqlWriter.WriteName(BuilderContext, Text, "", sf.StorageField.Name);
			}
			return node;
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
