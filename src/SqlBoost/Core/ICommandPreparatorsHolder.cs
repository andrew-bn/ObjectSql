﻿using SqlBoost.Core.Bo.CommandPreparatorDescriptor;
using System.Collections.Generic;

namespace SqlBoost.Core
{
	internal interface ICommandPreparatorsHolder
	{
		IReadOnlyList<CommandPreparator> Preparators { get; }
		int ParametersEncountered { get; set; }
		void AddPreparator(CommandPreparator preparator);
	}
}
