using System;
using System.Data;
using System.Data.Common;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;

namespace ObjectSql.Core.Bo
{
	public class QueryPreparationData
	{
		public CommandPrePostProcessor[] PreProcessors { get; private set; }
		public CommandPrePostProcessor[] PostProcessors { get; private set; }
		public string CommandText { get; private set; }
		public Delegate DataMaterializer { get; private set; }
		public Func<DbCommand, object> ReturnParameterReader { get; private set; }

		public QueryPreparationData(string commandText,
									CommandPrePostProcessor[] preProcessors,
									CommandPrePostProcessor[] postProcessors,
									Delegate dataMaterializer,
									Func<DbCommand, object> returnParameterReader)
		{
			CommandText = commandText;
			PreProcessors = preProcessors;
			PostProcessors = postProcessors;
			DataMaterializer = dataMaterializer;
			ReturnParameterReader = returnParameterReader;
		}
	}
}
