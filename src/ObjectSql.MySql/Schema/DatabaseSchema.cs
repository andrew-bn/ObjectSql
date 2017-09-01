namespace ObjectSql.MySql.Schema
{
	public class DatabaseSchema
	{
		public Table[] Tables { get; set; }
		public Procedure[] Procedures { get; set; }
		public Procedure[] Functions { get; set; }
	}
}