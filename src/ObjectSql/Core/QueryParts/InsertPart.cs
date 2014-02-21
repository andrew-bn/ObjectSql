using System.Linq.Expressions;
using ObjectSql.Core.Bo;

namespace ObjectSql.Core.QueryParts
{
	public class InsertPart : LambdaBasedQueryPart
	{
		public InsertPart(LambdaExpression expression)
			: base(expression)
		{

		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.Insert; }
		}

		public override void BuildPart(BuilderContext context)
		{
			var entity = Expression.Parameters[0].Type;
			var sql = context.ExpressionAnalizer.AnalizeExpression(context.Preparators, Expression.Body, ExpressionAnalizerType.FieldsSequence, false);
			context.SqlWriter.WriteInsert(context.Text, GetSchema(entity, context), sql);
			context.InsertionInfo = context.InsertionInfoExtractor.ExtractFrom(Expression);

		}
	}
}
