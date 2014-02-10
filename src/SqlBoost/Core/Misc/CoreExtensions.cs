using SqlBoost.Core.Bo.CommandPreparatorDescriptor;

namespace SqlBoost.Core.Misc
{
	internal static class CoreExtensions
	{
		public static InsertionParameterPreparator AsInsertion(this CommandPreparator descriptor)
		{
			return (InsertionParameterPreparator)descriptor;
		}
		public static DatabaseCommandParameterPreparator AsDatabaseParameter(this CommandPreparator descriptor)
		{
			return (DatabaseCommandParameterPreparator)descriptor;
		}
		public static SingleParameterPreparator AsSingleParameter(this CommandPreparator descriptor)
		{
			return (SingleParameterPreparator)descriptor;
		}

	}
}
