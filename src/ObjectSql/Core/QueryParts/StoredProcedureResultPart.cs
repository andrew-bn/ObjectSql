using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using ObjectSql.Core.Bo.EntitySchema;

namespace ObjectSql.Core.QueryParts
{
	public class StoredProcedureResultPart : QueryPart
	{
		public Type ResultType { get; private set; }
		public object DbType { get; private set; }

		public StoredProcedureResultPart(Type resultType, object dbType)
		{
			ResultType = resultType;
			DbType = dbType;
		}

		public override QueryPartType PartType
		{
			get { return QueryPartType.StoredProcedureReturn; }
		}

		public override bool IsEqualTo(QueryPart part, ref Bo.QueryRoots rootsA, ref Bo.QueryRoots rootsB)
		{
			return base.IsEqualTo(part, ref rootsA, ref rootsB) &&
			       (ResultType == ((StoredProcedureResultPart)part).ResultType);
		}

		public override void BuildPart(Bo.BuilderContext context)
		{
			var preparator = new SimpleCommandPrePostProcessor(context.DelegatesBuilder.AddCommandReturnParameter(ResultType, DbType));
			context.Preparators.AddPreProcessor(preparator);
		}
	}
}
