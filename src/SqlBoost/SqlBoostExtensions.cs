﻿using SqlBoost.Core;
using SqlBoost.Core.Bo;
using SqlBoost.QueryImplementation;
using SqlBoost.QueryInterfaces;
using System.Data;

namespace SqlBoost
{
	public static class SqlBoostExtensions
	{
		public static ISql Query(this IDbCommand command)
		{
			return command.Query(ResourcesTreatmentType.DisposeReader);
		}
		public static ISql Query(this IDbConnection connection)
		{
			return connection.CreateCommand().Query(ResourcesTreatmentType.DisposeCommand);
		}
		internal static ISql Query(this IDbCommand command, ResourcesTreatmentType treatType)
		{
			var sqlBoostCommand = command as SqlBoostCommand;
			var dbCommand = sqlBoostCommand == null ? command : sqlBoostCommand.UnderlyingCommand;
			var context = QueryManager.CreateQueryContext(dbCommand, command.Connection.ConnectionString, treatType);
			return new Sql(context);
		}
	}
}
