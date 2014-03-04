using System;
using System.Data;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;
using System.Collections.Generic;

namespace ObjectSql.Core
{
	public interface ICommandPreparatorsHolder
	{
		IList<CommandPrePostProcessor> PreProcessors { get; }
		IList<CommandPrePostProcessor> PostProcessors { get; }
		int ParametersEncountered { get; set; }
		void AddPreProcessor(CommandPrePostProcessor prePostProcessor);
		void AddPostProcessor(CommandPrePostProcessor prePostProcessor);
	}
}
