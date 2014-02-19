using ObjectSql.Core.Bo;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Bo.EntitySchema;
using ObjectSql.Core.QueryBuilder.ExpressionsAnalizers;
using ObjectSql.Core.QueryBuilder.InfoExtractor;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.QueryParts;
using ObjectSql.Core.SchemaManager;
using ObjectSql.Exceptions;
using ObjectSql.QueryInterfaces;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace ObjectSql.Core.QueryBuilder
{
	public class ObjectQueryBuilder: IQueryBuilder
	{
		private readonly IEntitySchemaManager _schemaManager;
		private readonly ISqlWriter _sqlWriter;
		private readonly IDelegatesBuilder _delegatesBuilder;
		private readonly IExpressionAnalizer _expressionAnalizer;
		private readonly IMaterializationInfoExtractor _materializationInfoExtrator;
		private readonly IInsertionInfoExtractor _insertionInfoExtractor;

		public ObjectQueryBuilder(QueryEnvironment env)
			: this(env.SchemaManager,env.SqlWriter,env.DelegatesBuilder,
					new ExpressionAnalizer(env.SchemaManager,env.DelegatesBuilder,env.SqlWriter),
					new MaterializationInfoExtractor(env.SchemaManager),
					new InsertionInfoExtractor(env.SchemaManager))
		{
		}

		private ObjectQueryBuilder(IEntitySchemaManager schemaManager,
							   ISqlWriter sqlWriter,
							   IDelegatesBuilder expressionBuilder,
							   IExpressionAnalizer expressionAnalizer,
							   IMaterializationInfoExtractor materializationInfoExtrator, 
							   IInsertionInfoExtractor insertionInfoExtractor)
		{
			_schemaManager = schemaManager;
			_sqlWriter = sqlWriter;
			_delegatesBuilder = expressionBuilder;
			_expressionAnalizer = expressionAnalizer;
			_materializationInfoExtrator = materializationInfoExtrator;
			_insertionInfoExtractor = insertionInfoExtractor;
		}

		public QueryPreparationData BuildQuery(IQueryPart[] parts)
		{
			var context = new BuilderContext(_schemaManager, _sqlWriter,_expressionAnalizer);

			foreach (var part in parts)
			{
				switch (part.PartType)
				{
						//select
					case QueryPartType.From: part.BuildPart(context); break;
					case QueryPartType.Join: part.BuildPart(context); break;
					case QueryPartType.Where: part.BuildPart(context); break;
					case QueryPartType.GroupBy: BuildGoupByPart((GroupByPart)part, context); break;
					case QueryPartType.Select: BuildSelectPart((SelectPart)part, context); break;

						//insert
					case QueryPartType.Insert: BuildInsertPart((InsertPart)part, context); break;
					case QueryPartType.Values: BuildValuesPart((ValuesPart)part, context); break;

						//delete
					case QueryPartType.Delete: BuildDeletePart((DeletePart)part, context); break;

						//update
					case QueryPartType.Update: BuildUpdatePart((UpdatePart)part, context); break;

						//sp
					case QueryPartType.StoredProcedure: BuildStoredProcedure((StoredProcedurePart)part, context); break;
				}
			}

			return new QueryPreparationData(context.Text.ToString(),
											context.Preparators.PreProcessors.ToArray(),
											context.Preparators.PostProcessors.ToArray(),
											context.MaterializationDelegate);
		}

		private void BuildStoredProcedure(StoredProcedurePart storedProcedurePart, BuilderContext context)
		{
			var exp = storedProcedurePart.Expression.Body;
			if (exp.NodeType == ExpressionType.Convert)
				exp = ((UnaryExpression)exp).Operand;
			if (exp.NodeType != ExpressionType.Call)
				throw new ObjectSqlException("Method call expression expected");

			var methodCall = ((MethodCallExpression)exp);

			var funcSchema = _schemaManager.GetFuncSchema(methodCall.Method);
			

			_sqlWriter.WriteProcedureCall(context.Text, funcSchema);

			var changeDbCommandType = _delegatesBuilder.CreateChangeDatabaseCommandTypeAction(CommandType.StoredProcedure);
			
			var param = new SimpleCommandPrePostProcessor(changeDbCommandType);
			context.Preparators.AddPreProcessor(param);

			if (storedProcedurePart.HasResultEntityType)
			{
				var entitySchema = _schemaManager.GetSchema(storedProcedurePart.EntityType);
				var indexes = entitySchema.EntityFields.Select(f => f.Index).ToArray();
				var materializationInfo = new EntityMaterializationInformation(indexes);
				context.MaterializationDelegate = _delegatesBuilder.CreateEntityMaterializationDelegate(entitySchema, materializationInfo);
			}

			_expressionAnalizer.AnalizeExpression(context.Preparators, methodCall, ExpressionAnalizerType.FuncCall, false);
		}

		private void BuildUpdatePart(UpdatePart updatePart, BuilderContext context)
		{
			var entity = updatePart.Expression.ReturnType;
			var sql = _expressionAnalizer.AnalizeExpression(context.Preparators, updatePart.Expression.Body, ExpressionAnalizerType.FieldsUpdate, false);
			_sqlWriter.WriteUpdate(context.Text, GetSchema(entity), sql);
		}

		private void BuildDeletePart(DeletePart deletePart, BuilderContext context)
		{
			_sqlWriter.WriteDelete(context.Text, GetSchema(deletePart.Entity));
		}

		#region insert
		private void BuildInsertPart(InsertPart insertPart, BuilderContext context)
		{
			var entity = insertPart.Expression.Parameters[0].Type;
			var sql = _expressionAnalizer.AnalizeExpression(context.Preparators, insertPart.Expression.Body,ExpressionAnalizerType.FieldsSequence, false);
			_sqlWriter.WriteInsert(context.Text, GetSchema(entity), sql);
			context.InsertionInfo = _insertionInfoExtractor.ExtractFrom(insertPart.Expression);
		}
		private void BuildValuesPart(ValuesPart valuesPart, BuilderContext context)
		{
			var entitySchema = GetSchema(valuesPart.Type);
			var insertionAction = _delegatesBuilder.CreateInsertionParametersInitializerAction(entitySchema, context.InsertionInfo);

			var param = new InsertionParameterPrePostProcessor(insertionAction);
			context.Preparators.AddPreProcessor(param);
		}
			#endregion
		#region select

		private void BuildJoinPart(JoinPart joinPart, BuilderContext context)
		{
			AppendAlias(joinPart.Expression, context);
			var joinToTable = joinPart.Expression.Parameters.Last().Type;
			var sql = _expressionAnalizer.AnalizeExpression(context.Preparators, joinPart.Expression.Body, ExpressionAnalizerType.Expression, true);
			_sqlWriter.WriteJoin(context.Text, GetSchema(joinToTable), joinPart.Expression.Parameters.Last().Name, sql);
		}

		private void BuildWherePart(WherePart wherePart, BuilderContext context)
		{
			if (wherePart.UseAliases)
				AppendAlias(wherePart.Expression, context);
			var sql = _expressionAnalizer.AnalizeExpression(context.Preparators, wherePart.Expression.Body, ExpressionAnalizerType.Expression, wherePart.UseAliases);
			if (context.State == BuilderState.GroupByGenerated)
				_sqlWriter.WriteHaving(context.Text, sql);
			else
				_sqlWriter.WriteWhere(context.Text, sql);
		}

		private void BuildGoupByPart(GroupByPart groupByPart, BuilderContext context)
		{
			AppendAlias(groupByPart.Expression, context);
			var sql = _expressionAnalizer.AnalizeExpression(context.Preparators, groupByPart.Expression.Body, ExpressionAnalizerType.FieldsSequence, true);
			_sqlWriter.WriteGroupBy(context.Text, sql);
			context.State = BuilderState.GroupByGenerated;
		}
		private void BuildSelectPart(SelectPart selectPart, BuilderContext context)
		{
			AppendAlias(selectPart.Expression, context);

			var buff = context.Text;
			context.Text = new CommandText();
			var sql = _expressionAnalizer.AnalizeExpression(context.Preparators, selectPart.Expression.Body, ExpressionAnalizerType.FieldsSelect, true);
			_sqlWriter.WriteSelect(context.Text, sql);
			context.Text.Append(buff.ToString());

			var matInfo = _materializationInfoExtrator.ExtractFrom(selectPart.Expression.Body);

			context.MaterializationDelegate = _delegatesBuilder.CreateEntityMaterializationDelegate(
				GetSchema(selectPart.Expression.ReturnType), matInfo);
		}
		#endregion
		
		#region helpers
		
		private void AppendAlias(LambdaExpression expression, BuilderContext context)
		{
			if (context.State == BuilderState.FromAliasNeeded &&
				context.State != BuilderState.FromAliasGenerated &&
				context.State != BuilderState.GroupByGenerated)
			{
				if (expression.Parameters[0].Type != typeof (DatabaseExtension))
					_sqlWriter.WriteAlias(context.Text, expression.Parameters[0].Name);
				else _sqlWriter.WriteAlias(context.Text, expression.Parameters[1].Name);
				context.State = BuilderState.FromAliasGenerated;
			}
		}
		public EntitySchema GetSchema(Type entityType)
		{
			return _schemaManager.GetSchema(entityType);
		}
		#endregion
	}
}
