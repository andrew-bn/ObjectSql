using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjectSql.Core.QueryBuilder;
using ObjectSql.Core.QueryBuilder.LambdaBuilder;
using ObjectSql.Core.SchemaManager;

namespace ObjectSql.Core.Bo
{
	public class QueryEnvironment
	{
		public string InitialConnectionString { get; private set; }
		public IDbCommand Command { get; private set; }
		public ResourcesTreatmentType ResourcesTreatmentType { get; set; }
		public IEntitySchemaManager SchemaManager { get; private set; }
		public IDelegatesBuilder DelegatesBuilder { get; private set; }
		public ISqlWriter SqlWriter { get; private set; }

		public QueryEnvironment(string initialConnectionString,
								IDbCommand command,
								ResourcesTreatmentType resourcesTreatmentType,
								IEntitySchemaManager schemaManager,
								IDelegatesBuilder delegatesBuilder,
								ISqlWriter sqlWriter)
		{
			InitialConnectionString = initialConnectionString;
			Command = command;
			ResourcesTreatmentType = resourcesTreatmentType;
			SchemaManager = schemaManager;
			DelegatesBuilder = delegatesBuilder;
			SqlWriter = sqlWriter;
		}
	}
}
