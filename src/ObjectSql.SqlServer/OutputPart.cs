using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Misc;
using ObjectSql.Core.QueryParts;

namespace ObjectSql.SqlServer
{
	public class OutputPart: LambdaBasedQueryPart
	{
		public OutputPart(LambdaExpression expression)
			:base(expression)
		{
			
		}

		public override void BuildPart(BuilderContext context)
		{
			var expr = (LambdaExpression)new OutputClauseParameterSubstitutor(Expression.Parameters.ToArray()).Visit(Expression);

			var sql = context.AnalizeExpression(expr.Parameters.ToArray(), expr.Body, ExpressionAnalizerType.FieldsSelect, true);
			((SqlServerSqlWriter)context.SqlWriter).WriteOutput(context.Text, sql);

			var matInfo = context.MaterializationInfoExtractor.ExtractFrom(expr.Body);

			context.MaterializationDelegate = context.DelegatesBuilder.CreateEntityMaterializationDelegate(
				GetSchema(expr.ReturnType, context), matInfo);
		}
	}

	public class OutputClauseParameterSubstitutor : ExpressionVisitor
	{
		private readonly ParameterExpression[] _parameters;

		public OutputClauseParameterSubstitutor(ParameterExpression[] parameters)
		{
			_parameters = parameters;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == _parameters[0])
				return Expression.Parameter(node.Type, "INSERTED");
			if (node == _parameters[1])
				return Expression.Parameter(node.Type, "DELETED");

			return node;
		}
	}
}
