using System;
using System.Collections.Generic;
using System.Data;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;

namespace ObjectSql.Core.Bo
{
	public class CommandPreparatorsHolder : ICommandPreparatorsHolder
	{
		private readonly List<CommandPrePostProcessor> _preProcessors;
		private readonly List<CommandPrePostProcessor> _postProcessors;
		public int ParametersEncountered { get; set; }
		public IList<CommandPrePostProcessor> PreProcessors
		{
			get { return _preProcessors; }
		}
		public IList<CommandPrePostProcessor> PostProcessors
		{
			get { return _postProcessors; }
		}
		public CommandPreparatorsHolder()
		{
			_preProcessors = new List<CommandPrePostProcessor>();
			_postProcessors = new List<CommandPrePostProcessor>();
		}

		public void AddPreProcessor(CommandPrePostProcessor prePostProcessor)
		{
			_preProcessors.Add(prePostProcessor);
		}

		public void AddPostProcessor(CommandPrePostProcessor prePostProcessor)
		{
			_postProcessors.Add(prePostProcessor);
		}

		public Func<IDbCommand, object> ReturnParameterReader { get; set; }
	}
}
