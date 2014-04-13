using ObjectSql.Core.SchemaManager;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.InfoExtractor
{
	public class MaterializationInfoExtractor : ExpressionVisitor, IMaterializationInfoExtractor
	{
		private readonly IEntitySchemaManager _schemaManager;
		private EntityMaterializationInformation _result;

		public MaterializationInfoExtractor(IEntitySchemaManager schemaManager)
		{
			_schemaManager = schemaManager;
		}

		public EntityMaterializationInformation ExtractFrom(Expression expression)
		{
			_result = null;
			Visit(expression);
			return _result;
		}
		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (_result == null)
				_result = new EntityMaterializationInformation(node.Type);
			return node;
		}
		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (_result == null)
				_result = new EntityMaterializationInformation(node.Type);
			return node;
		}
		protected override Expression VisitMember(MemberExpression node)
		{
			if (_result == null)
				_result = new EntityMaterializationInformation(node.Type);
			return node;
		}

		protected override Expression VisitParameter(ParameterExpression alias)
		{
			if (_result == null)
			{
				var entitySchema = _schemaManager.GetSchema(alias.Type);
				var entityFields = entitySchema.EntityFields;
				var indexes = entityFields.Select(f => f.Index).ToArray();
				_result = new EntityMaterializationInformation(indexes);
			}
			return alias;
		}

		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			if (_result == null)
			{
				var entitySchema = _schemaManager.GetSchema(node.Type);
				var indexes = node.Bindings.Select(b => b as MemberAssignment).Where(b => b != null)
							 .Select(b => entitySchema.GetEntityPropertyByName(b.Member.Name).Index)
							 .ToArray();

				_result = new EntityMaterializationInformation(indexes);
			}
			return node;
		}
		protected override Expression VisitNew(NewExpression node)
		{
			if (_result == null)
				_result = new EntityMaterializationInformation(node.Constructor);
			return node;
		}
	}
}
