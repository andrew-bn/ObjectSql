using SqlBoost.Core.SchemaManager;
using System.Linq;
using System.Linq.Expressions;

namespace SqlBoost.Core.QueryBuilder.InfoExtractor
{
	internal class InsertionInfoExtractor : ExpressionVisitor,IInsertionInfoExtractor
	{
		private readonly IEntitySchemaManager _schemaManager;
		private EntityInsertionInformation _result;

		public InsertionInfoExtractor(IEntitySchemaManager schemaManager)
		{
			_schemaManager = schemaManager;
		}

		public EntityInsertionInformation ExtractFrom(Expression expression)
		{
			_result = null;
			Visit(expression);
			return _result;
		}

		protected override Expression VisitParameter(ParameterExpression alias)
		{
			if (_result == null)
			{
				var entitySchema = _schemaManager.GetSchema(alias.Type);
				var entityFields = entitySchema.EntityFields;
				var propsIndexes = entityFields.Select(f => f.Index).ToArray();
				_result = new EntityInsertionInformation(propsIndexes);
			}
			return alias;
		}
		
		protected override Expression VisitNew(NewExpression node)
		{
			if (_result == null)
			{
				var indexes = node.Arguments.Select(a => a as MemberExpression).Where(a => a != null)
							 .Select(a => _schemaManager.GetSchema(a.Expression.Type).GetEntityPropertyByName(a.Member.Name).Index)
							 .ToArray();

				_result = new EntityInsertionInformation(indexes);
			}
			return node;
		}
	}
}
