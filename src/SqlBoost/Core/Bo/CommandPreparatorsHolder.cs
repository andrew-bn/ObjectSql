using System.Collections.Generic;

namespace SqlBoost.Core.Bo
{
	internal class CommandPreparatorsHolder : ICommandPreparatorsHolder
	{
		public CommandPreparatorsHolder()
		{
			_parametersDescriptors = new List<CommandPreparatorDescriptor.CommandPreparator>();
		}
		private readonly List<CommandPreparatorDescriptor.CommandPreparator> _parametersDescriptors;

		public int ParametersEncountered { get; set; }

		public IReadOnlyList<CommandPreparatorDescriptor.CommandPreparator> Preparators
		{
			get { return _parametersDescriptors; }
		}

		public void AddPreparator(CommandPreparatorDescriptor.CommandPreparator preparator)
		{
			_parametersDescriptors.Add(preparator);
		}
	}
}
