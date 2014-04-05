using System.Data.Common;
using ObjectSql.Core.Bo;
using ObjectSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core
{
	internal static class ExecutionManager
	{
		private static void PrepareQuery(QueryContext context)
		{
			context.PrepareQuery();
		}

		public static IScalarResultHolder ExecuteScalar(QueryContext context)
		{
			var result = ExecuteCommand(context, cmd => cmd.ExecuteScalar(), true);
			return new ScalarResultHolder(context,result);
		}

		public static INonQueryResultHolder ExecuteNonQuery(QueryContext context)
		{
			var result = ExecuteCommand(context, cmd => cmd.ExecuteNonQuery(), true);
			return new NonQueryResultHolder(context,result);
		}

		public static IEnumerable<T> ExecuteQuery<T>(QueryContext context)
		{
			var dataReader = ExecuteDataReader(context);
			return new EntityEnumerable<T>(context.MaterializationDelegate, dataReader, () => DisposeDataReader(context, dataReader));
		}

		public static IDataReaderHolder ExecuteReader<T>(QueryContext context)
		{
			var dataReader = ExecuteDataReader(context);
			return new DataReaderHolder(context, dataReader, () => DisposeDataReader(context, dataReader));
		}

		private static IDataReader ExecuteDataReader(QueryContext context)
		{
			return ExecuteCommand(context, c => c.ExecuteReader(), false);
		}

		private static T ExecuteCommand<T>(QueryContext context, Func<IDbCommand, T> executor, bool freeResources)
		{
			PrepareCommand(context);
			try
			{
				return executor(context.Command);
			}
			catch
			{
				freeResources = true;
				throw;
			}
			finally
			{
				if (freeResources)
					FreeResources(context);
			}
		}

		private static void PrepareCommand(QueryContext context)
		{
			PrepareQuery(context);
			context.ConnectionOpened = OpenConnection(context.Command.Connection);
		}

		private static void RunPostProcessors(QueryContext context)
		{
			var postProcessors = context.PreparationData.PostProcessors;
			for (int i = 0; i < postProcessors.Length; i++)
			{
				var prc = postProcessors[i];
				if (!prc.RootDemanding)
					prc.CommandPreparationAction(context.Command, null);
				else
					prc.CommandPreparationAction(context.Command, context.SqlPart.QueryRoots.Roots[prc.RootIndex]);
			}
		}

		private static void FreeResources(QueryContext context)
		{
			RunPostProcessors(context);

			var cmd = context.Command;
			var connectionOpened = context.ConnectionOpened;

			if (connectionOpened)
				cmd.Connection.Close();
			if (context.ResourcesTreatmentType == ResourcesTreatmentType.DisposeCommand ||
				context.ResourcesTreatmentType == ResourcesTreatmentType.DisposeConnection)
				cmd.Dispose();
			if (context.ResourcesTreatmentType == ResourcesTreatmentType.DisposeConnection)
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
			if (!(context.Command is DbCommand))
				throw new ObjectSqlException("Provider does not support async operations");

			PrepareQuery(context);

			var cmd = (DbCommand)context.Command;
			var connection = cmd.Connection;
			var connectionOpened = connection.State == ConnectionState.Closed;
			if (connectionOpened) await connection.OpenAsync();

			var dataReader = await cmd.ExecuteReaderAsync();

			return new EntityEnumerable<T>(context.MaterializationDelegate, dataReader, () => DisposeDataReader(context, dataReader));
		}


		#endregion async
#endif
		private static void DisposeDataReader(QueryContext context, IDataReader dataReader)
		{
			dataReader.Dispose();
			FreeResources(context);
		}
	}
}
