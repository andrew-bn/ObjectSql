namespace SqlBoost.Core
{
	public class EntityInsertionInformation
	{
		public int[] PropertiesIndexesToInsert { get; private set; }

		public EntityInsertionInformation(int[] propertiesIndexesToInsert)
		{
			PropertiesIndexesToInsert = propertiesIndexesToInsert;
		}
	}
}
