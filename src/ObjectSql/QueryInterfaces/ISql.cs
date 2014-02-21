namespace ObjectSql.QueryInterfaces
{
	public interface ISql: IQueryContextHolder, ISelectRoot, IInsertRoot, IUpdateRoot, IDeleteRoot, IStoredProcedureRoot
	{
	}
}
