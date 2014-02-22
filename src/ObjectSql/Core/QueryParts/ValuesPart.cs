using System;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;

namespace ObjectSql.Core.QueryParts
{
	public class ValuesPart: QueryPart
	{
		public object[] Values { get; private set; }
		public Type Type { get; private set; }
		public ValuesPart(Type type, object[] values)
		{
			Values = values;
			Type = type;
		}

		public override void CalculateQueryExpressionParameters(ref QueryRoots parameters)
		{
			base.CalculateQueryExpressionParameters(ref parameters);
			parameters.AddRoot(Values);
		}

		public override void BuildPart(BuilderContext context)
		{
			var entitySchema = GetSchema(Type,context);
			var insertionAction = context.DelegatesBuilder.CreateInsertionParametersInitializerAction(entitySchema, context.InsertionInfo);

			var param = new InsertionParameterPrePostProcessor(insertionAction);
			context.Preparators.AddPreProcessor(param);
		}
	}
}
