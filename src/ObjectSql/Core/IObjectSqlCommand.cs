﻿using System.Data;
using System.Data.Common;

namespace ObjectSql.Core
{
	public interface IObjectSqlCommand
	{
		DbCommand UnderlyingCommand { get; }
	}
}
