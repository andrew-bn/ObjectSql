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


		public override bool SortParts(BuilderContext context)
		{
			var parts = context.Parts;
			var index = parts.IndexOf(this);
			for (int i = index-1; i >= 0; i--)
			{
				if (i == 0 || (parts[i] is NextQueryPart) && i != index)
				{
					parts.RemoveAt(index);
					parts.Insert(i, this);
					return true;
				}
			}
			return false;
		}

		public override void BuildPart(BuilderContext context)
		{
			var sql = context.ExpressionAnalizer.AnalizeExpression(context.Preparators, Expression.Body, ExpressionAnalizerType.FieldsSelect, true);
			context.SqlWriter.WriteSelect(context.Text, sql);

			var matInfo = context.MaterializationInfoExtractor.ExtractFrom(Expression.Body);

			context.MaterializationDelegate = context.DelegatesBuilder.CreateEntityMaterializationDelegate(
				GetSchema(Expression.ReturnType,context), matInfo);

		}
	}
}
