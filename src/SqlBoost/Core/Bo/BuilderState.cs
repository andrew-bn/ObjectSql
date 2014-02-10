namespace SqlBoost.Core.Bo
{
	internal enum BuilderState
	{
		None,
		FromAliasNeeded,
		FromAliasGenerated,
		GroupByGenerated
	}
}
