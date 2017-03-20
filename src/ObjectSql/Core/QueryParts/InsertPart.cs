using System.Linq;
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

		public override void BuildPart(BuilderContext context)
		{
			var entity = Expression.Parameters[0].Type;
			var sql = context.AnalizeExpression(Expression.Parameters.ToArray(), Expression.Body, ExpressionAnalizerType.FieldsSequence);
			context.SqlWriter.WriteInsert(context.Text, GetSchema(entity, context), sql);
			context.InsertionInfo = context.InsertionInfoExtractor.ExtractFrom(Expression);
		}
	}
}
