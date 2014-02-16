using SqlBoost.Core.Bo.CommandPreparatorDescriptor;
using System.Collections.Generic;

namespace SqlBoost.Core
{
	public interface ICommandPreparatorsHolder
	{
		IList<CommandPreparator> Preparators { get; }
		int ParametersEncountered { get; set; }
		void AddPreparator(CommandPreparator preparator);
	}
}
