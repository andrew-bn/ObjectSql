namespace SqlBoost.Core.Bo
{
	public enum BuilderState
	{
		None,
		FromAliasNeeded,
		FromAliasGenerated,
		GroupByGenerated
	}
}
