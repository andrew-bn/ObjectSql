using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class SelectPart: LambdaBasedQueryPart
	{
		public SelectPart(LambdaExpression expression)
			:base(expression)
		{
			
		}



		public override void BuildPart(BuilderContext context)
		{
			AppendAlias(Expression, context);

			var buff = context.Text;
			context.Text = new CommandText();
			var sql = context.ExpressionAnalizer.AnalizeExpression(context.Preparators, Expression.Body, ExpressionAnalizerType.FieldsSelect, true);
			context.SqlWriter.WriteSelect(context.Text, sql);
			context.Text.Append(buff.ToString());

			var matInfo = context.MaterializationInfoExtractor.ExtractFrom(Expression.Body);

			context.MaterializationDelegate = context.DelegatesBuilder.CreateEntityMaterializationDelegate(
				GetSchema(Expression.ReturnType,context), matInfo);

		}
	}
}
