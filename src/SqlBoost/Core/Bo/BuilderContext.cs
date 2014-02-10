using System;

namespace SqlBoost.Core.Bo
{
	internal class BuilderContext
	{
		public BuilderContext()
		{
			Text = new CommandText();
			Preparators = new CommandPreparatorsHolder();
		}
		public CommandPreparatorsHolder Preparators { get; private set; }
		public BuilderState State { get; set; }
		public CommandText Text { get; set; }
		public EntityInsertionInformation InsertionInfo { get; set; }
		public EntityMaterializationInformation MaterializationInfo { get; set; }
		public Delegate MaterializationDelegate { get; set; }
	}
}
