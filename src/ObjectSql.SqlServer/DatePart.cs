using ObjectSql.QueryInterfaces;

namespace ObjectSql.SqlServer
{
	[DatabaseExtension]
	public enum DatePart
	{
		[Emit("year")]
		Year,

		[Emit("quarter")]
		Quarter,

		[Emit("month")]
		Month,

		[Emit("dayofyear")]
		DayOfYear,

		[Emit("day")]
		Day,

		[Emit("week")]
		Week,

		[Emit("hour")]
		Hour,

		[Emit("minute")]
		Minute,

		[Emit("second")]
		Second,

		[Emit("millisecond")]
		Millisecond,

		[Emit("microsecond")]
		Microsecond,

		[Emit("nanosecond")]
		Nanosecond,
	}
}