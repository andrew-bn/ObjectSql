using System;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;

namespace ObjectSql.Core.Bo
{
	public class QueryPreparationData
	{
		public CommandPreparator[] Parameters { get; private set; }
		public string CommandText { get; private set; }
		public Delegate DataMaterializer { get; private set; }

		public QueryPreparationData(string commandText,
									  CommandPreparator[] parameters,
									  Delegate dataMaterializer)
		{
			CommandText = commandText;
			Parameters = parameters;
			DataMaterializer = dataMaterializer;
		}
	}
}
