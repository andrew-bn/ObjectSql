namespace SqlBoost.QueryInterfaces
{
	public interface ISql: IQueryContextHolder, ISelectRoot, IInsertRoot, IUpdateRoot, IDeleteRoot, IStoredProcedure
	{
	}
}
