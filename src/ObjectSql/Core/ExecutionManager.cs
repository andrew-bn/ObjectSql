using System.Data.Common;
using System.Linq;
using System.Reflection;
using ObjectSql.Core.Bo;
using ObjectSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ObjectSql.QueryImplementation;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core
{
	internal static class ExecutionManager
	{
		private static void PrepareQuery(QueryContext context)
		{
			QueryManager.PrepareQuery(context);
		}
		public static object ExecuteScalar(QueryContext context)
		{
			PrepareQuery(context);
			return ExecuteCommand(context, cmd => cmd.ExecuteScalar(), true);
		}
		public static int ExecuteNonQuery(QueryContext context)
		{
			PrepareQuery(context);
			return ExecuteCommand(context, cmd => cmd.ExecuteNonQuery(), true);
		}
		public static IEnumerable<T> ExecuteQuery<T>(QueryContext context)
		{
			PrepareQuery(context);
			var cmd = context.QueryEnvironment.Command;

			var connectionOpened = OpenConnection(cmd.Connection);

			var dataReader = ExecuteCommand(context, c => c.ExecuteReader(), false);

			return new EntityEnumerable<T>(context.MaterializationDelegate, dataReader, () => DisposeDataReader(context, dataReader, connectionOpened));
		}
		public static IQueryDataReader ExecuteReader(QueryContext context)
		{
			PrepareQuery(context);
			var cmd = context.QueryEnvironment.Command;

			var connectionOpened = OpenConnection(cmd.Connection);

			var dataReader = ExecuteCommand(context, c => c.ExecuteReader(), false);

			return new QueryDataReader(context, dataReader, () => DisposeDataReader(context, dataReader, connectionOpened));
		}

		private static T ExecuteCommand<T>(QueryContext context, Func<IDbCommand, T> executor, bool freeResources)
		{
			var cmd = context.QueryEnvironment.Command;
			var connectionOpened = OpenConnection(cmd.Connection);
			try
			{
				var result = executor(cmd);
				return result;
			}
			finally
			{
				if (freeResources)
					FreeResources(context, cmd, connectionOpened);
			}
		}

		private static void RunPostProcessors(QueryContext context)
		{
			var postProcessors = context.PreparationData.PostProcessors;
			for (int i = 0; i < postProcessors.Length; i++)
			{
				if (!postProcessors[i].RootDemanding)
					postProcessors[i].CommandPreparationAction(context.QueryEnvironment.Command, null);
				else
				{
					foreach (var root in context.QueryRoots.Roots)
					{
						if ((root.Value & postProcessors[i].RootMap) != 0)
							postProcessors[i].CommandPreparationAction(context.QueryEnvironment.Command, root.Key);
					}
				}
			}
		}

		private static void FreeResources(QueryContext context, IDbCommand cmd, bool connectionOpened)
		{
			RunPostProcessors(context);

			if (connectionOpened)
				cmd.Connection.Close();
			if (context.QueryEnvironment.ResourcesTreatmentType == ResourcesTreatmentType.DisposeCommand ||
				context.QueryEnvironment.ResourcesTreatmentType == ResourcesTreatmentType.DisposeConnection)
				cmd.Dispose();
			if (context.QueryEnvironment.ResourcesTreatmentType == ResourcesTreatmentType.DisposeConnection)
				cmd.Connection.Dispose();
		}

		private static bool OpenConnection(IDbConnection dbConnection)
		{
			bool openConnection = dbConnection.State == ConnectionState.Closed;
			if (openConnection)
				dbConnection.Open();
			return openConnection;
		}
#if NET45
		#region async
		public static async Task<IAsyncEnumerable<T>> ExecuteQueryAsync<T>(QueryContext context)
		{
			if (!(context.QueryEnvironment.Command is DbCommand))
				throw new ObjectSqlException("Provider does not support async operations");

			PrepareQuery(context);

			var cmd = (DbCommand)context.QueryEnvironment.Command;
			var connection = cmd.Connection;
			var connectionOpened = connection.State == ConnectionState.Closed;
			if (connectionOpened) await connection.OpenAsync();

			var dataReader = await cmd.ExecuteReaderAsync();

			return new EntityEnumerable<T>(context.MaterializationDelegate, dataReader, () => DisposeDataReader(context, dataReader, connectionOpened));
		}


		#endregion async
#endif
		private static void DisposeDataReader(QueryContext context, IDataReader dataReader, bool connectionOpened)
		{
			dataReader.Dispose();
			FreeResources(context, context.QueryEnvironment.Command, connectionOpened);
		}
	}
}
