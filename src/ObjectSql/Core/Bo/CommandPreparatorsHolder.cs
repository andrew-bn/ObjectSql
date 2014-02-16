using System.Collections.Generic;

namespace ObjectSql.Core.Bo
{
	public class CommandPreparatorsHolder : ICommandPreparatorsHolder
	{
		public CommandPreparatorsHolder()
		{
			_parametersDescriptors = new List<CommandPreparatorDescriptor.CommandPreparator>();
		}
		private readonly List<CommandPreparatorDescriptor.CommandPreparator> _parametersDescriptors;

		public int ParametersEncountered { get; set; }

		public IList<CommandPreparatorDescriptor.CommandPreparator> Preparators
		{
			get { return _parametersDescriptors; }
		}

		public void AddPreparator(CommandPreparatorDescriptor.CommandPreparator preparator)
		{
			_parametersDescriptors.Add(preparator);
		}
	}
}
