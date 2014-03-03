using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Exceptions;

namespace ObjectSql.Core.QueryParts
{
	public class StoredProcedurePart : LambdaBasedQueryPart
	{
		public bool ReturnsCollection { get; private set; }
		public Type EntityType { get; private set; }
		public bool HasResultEntityType { get { return EntityType != null; } }
		public StoredProcedurePart(LambdaExpression expression, Type entityType, bool returnsCollection)
			: base(expression)
		{
			ReturnsCollection = returnsCollection;
			EntityType = entityType;
		}

		public override void BuildPart(Bo.BuilderContext context)
		{
			var exp = Expression.Body;
			if (exp.NodeType == ExpressionType.Convert)
				exp = ((UnaryExpression)exp).Operand;
			if (exp.NodeType != ExpressionType.Call)
				throw new ObjectSqlException("Method call expression expected");

			var methodCall = ((MethodCallExpression)exp);

			var funcSchema = context.SchemaManager.GetFuncSchema(methodCall.Method);


			context.SqlWriter.WriteProcedureCall(context.Text, funcSchema);

			var changeDbCommandType = context.DelegatesBuilder.CreateChangeDatabaseCommandTypeAction(CommandType.StoredProcedure);

			var param = new SimpleCommandPrePostProcessor(changeDbCommandType);
			context.Preparators.AddPreProcessor(param);

			if (HasResultEntityType)
			{
				var entitySchema = GetSchema(EntityType, context);
				var indexes = entitySchema.EntityFields.Select(f => f.Index).ToArray();
				var materializationInfo = new EntityMaterializationInformation(indexes);
				context.MaterializationDelegate = context.DelegatesBuilder.CreateEntityMaterializationDelegate(entitySchema, materializationInfo);
			}

			context.AnalizeExpression(Expression.Parameters.ToArray(), methodCall, ExpressionAnalizerType.FuncCall, false);
		}
	}
}
