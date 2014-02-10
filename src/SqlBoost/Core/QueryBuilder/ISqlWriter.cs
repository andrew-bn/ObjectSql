using SqlBoost.Core.Bo.EntitySchema;

namespace SqlBoost.Core.QueryBuilder
{
	internal interface ISqlWriter
	{
		CommandText WriteUpdate(CommandText commandText, EntitySchema entity, string updateSql);
		CommandText WriteDelete(CommandText commandText, EntitySchema entity);
		CommandText WriteInsert(CommandText commandText, EntitySchema entity, string fieldsSql);
		CommandText WriteFrom(CommandText commandText, EntitySchema entity);
		CommandText WriteAlias(CommandText commandText, string aliasName);
		CommandText WriteJoin(CommandText commandText, EntitySchema entity, string alias, string conditionSql);
		CommandText WriteWhere(CommandText commandText, string sql);
		CommandText WriteHaving(CommandText commandText, string sql);
		CommandText WriteGroupBy(CommandText commandText, string sql);
		CommandText WriteSelect(CommandText commandText, string sql);
		CommandText WriteProcedureCall(CommandText commandText, FuncSchema func);
		CommandText WriteParameter(CommandText commandText, string parameterName);
		CommandText WriteNull(CommandText commandText);
		CommandText WriteComma(CommandText commandText);
		CommandText WriteName(CommandText commandText, string name);
		CommandText WriteNameResolve(CommandText commandText);
		CommandText WriteBlock(CommandText commandText, string expression);
		CommandText WriteNot(CommandText commandText, string condition);
		CommandText WriteGreater(CommandText commandText, string left, string right);
		CommandText WriteGreaterOrEqual(CommandText commandText, string left, string right);
		CommandText WriteLess(CommandText commandText, string left, string right);
		CommandText WriteLessOrEqual(CommandText commandText, string left, string right);
		CommandText WriteAnd(CommandText commandText, string left, string right);
		CommandText WriteOr(CommandText commandText, string left, string right);
		CommandText WriteAdd(CommandText commandText, string left, string right);
		CommandText WriteSubtract(CommandText commandText, string left, string right);
		CommandText WriteDivide(CommandText commandText, string left, string right);
		CommandText WriteMultiply(CommandText commandText, string left, string right);
		CommandText WriteEqual(CommandText commandText, string left, string right);
		CommandText WriteEqualNull(CommandText commandText, string left);
		CommandText WriteNotEqual(CommandText commandText, string left, string right);
		CommandText WriteNotEqualNull(CommandText commandText, string left);

		CommandText WriteSet(CommandText commandText);
	}
}
