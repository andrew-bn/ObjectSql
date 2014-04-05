using ObjectSql.Core.Bo;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Exceptions;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder.ExpressionsAnalizers
{
	public class QuerySelectBuilder : QueryExpressionBuilder
	{
		private bool _multiFieldSelectionFromTable = true;		
		public QuerySelectBuilder(IEntitySchemaManager schemaManager, IDelegatesBuilder expressionBuilder, SqlWriter sqlWriter)
			: base(schemaManager, expressionBuilder, sqlWriter)
		{
		}
		public override string BuildSql(BuilderContext context, ParameterExpression[] parameters, Expression expression)
		{
			_multiFieldSelectionFromTable = true;
			return base.BuildSql(context,parameters, expression);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			_multiFieldSelectionFromTable = false;
			return base.VisitMember(node);
		}
		protected override Expression VisitParameter(ParameterExpression alias)
		{
			if (_multiFieldSelectionFromTable)
			{
				var entitySchema = SchemaManager.GetSchema(alias.Type);
				var entityFields = entitySchema.EntityFields;

				var appendComma = false;
				foreach (var fld in entityFields)
				{
					if (appendComma) SqlWriter.WriteComma(Text);
					appendComma = true;
					SqlWriter.WriteName(BuilderContext, Text, alias.Name, fld.StorageField.Name);
				}
			}
			else base.VisitParameter(alias);

			return alias;
		}
		
		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			if (node.NewExpression.Constructor.GetParameters().Length > 0)
				throw new ObjectSqlException("You can use either Object Initializer or Constructor with parameters to create entity instance");

			if (!_multiFieldSelectionFromTable)
				throw new ObjectSqlException("Complex types are not allowed");

			_multiFieldSelectionFromTable = false;

			for (int i = 0; i < node.Bindings.Count; i++)
			{
				if (i > 0) SqlWriter.WriteComma(Text);

				if (node.Bindings[i].BindingType == MemberBindingType.Assignment)
				{
					var assignment = (MemberAssignment)node.Bindings[i];
					Visit(assignment.Expression);
					SqlWriter.WriteAlias(Text, assignment.Member.Name);
				}
			}
			return node;
		}
		protected override Expression VisitNew(NewExpression node)
		{
			if (!_multiFieldSelectionFromTable)
				throw new ObjectSqlException("Complex types are not allowed");
			_multiFieldSelectionFromTable = false;
			var ctorParametersInfo = node.Members == null
							? node.Constructor.GetParameters()
							: null;

			for (int i = 0; i < node.Arguments.Count; i++)
			{
				if (node.Arguments[i].NodeType == ExpressionType.Parameter)
					throw new ObjectSqlException("In anonimus type you should provide only single fields, not whole classes");

				if (i > 0) SqlWriter.WriteComma(Text);
				
				Visit(node.Arguments[i]);

				SqlWriter.WriteAlias(Text, ctorParametersInfo == null ? node.Members[i].Name : ctorParametersInfo[i].Name);
			}
			return node;
		}
	}
}
