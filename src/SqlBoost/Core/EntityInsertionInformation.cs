namespace SqlBoost.Core
{
	internal class EntityInsertionInformation
	{
		public int[] PropertiesIndexesToInsert { get; private set; }

		public EntityInsertionInformation(int[] propertiesIndexesToInsert)
		{
			PropertiesIndexesToInsert = propertiesIndexesToInsert;
		}
	}
}
