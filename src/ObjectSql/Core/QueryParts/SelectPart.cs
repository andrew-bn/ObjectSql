using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Misc;

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
			var fromPart = context.Parts.MoveBackAndFind(this, p => p is FromPart) as FromPart;
			if (fromPart != null)
			{
				var selectIndex = context.Parts.IndexOf(this);
				var fromIndex = context.Parts.IndexOf(fromPart);
				parts.RemoveAt(selectIndex);
				parts.Insert(fromIndex,this);
				return true;
			}

			return false;
		}

		public override void BuildPart(BuilderContext context)
		{
			var sql = context.AnalizeExpression(Expression.Parameters.ToArray(), Expression.Body, ExpressionAnalizerType.FieldsSelect);
			context.SqlWriter.WriteSelect(context.Text, sql);

			var matInfo = context.MaterializationInfoExtractor.ExtractFrom(Expression.Body);

			context.MaterializationDelegate = context.DelegatesBuilder.CreateEntityMaterializationDelegate(
				GetSchema(Expression.ReturnType,context), matInfo);

		}
	}
}
