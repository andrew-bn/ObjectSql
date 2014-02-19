using System.Linq.Expressions;
using ObjectSql.Core.Bo.CommandPreparatorDescriptor;

namespace ObjectSql.Core.Misc
{
	internal static class CoreExtensions
	{
		public static InsertionParameterPrePostProcessor AsInsertion(this CommandPrePostProcessor descriptor)
		{
			return (InsertionParameterPrePostProcessor)descriptor;
		}
		public static DatabaseCommandParameterPrePostProcessor AsDatabaseParameter(this CommandPrePostProcessor descriptor)
		{
			return (DatabaseCommandParameterPrePostProcessor)descriptor;
		}
		public static StoredProcedureOutParameterProcessor AsStoredProcedureOutParameterProcessor(this CommandPrePostProcessor descriptor)
		{
			return (StoredProcedureOutParameterProcessor)descriptor;
		}

		public static SingleParameterPrePostProcessor AsSingleParameter(this CommandPrePostProcessor descriptor)
		{
			return (SingleParameterPrePostProcessor) descriptor;
		}
		public static Expression StripConvert(this Expression exp)
		{
			if (exp.NodeType == ExpressionType.Convert)
				return ((UnaryExpression) exp).Operand;
			return exp;
		}
	}
}
