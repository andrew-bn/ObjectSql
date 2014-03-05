using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ObjectSql.Core.Bo;
using ObjectSql.Exceptions;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core
{
	internal class DataReaderHolder : IDataReaderHolder
	{
		private static readonly ConcurrentDictionary<Type, Delegate> _mapMaterializers = new ConcurrentDictionary<Type, Delegate>();

		private readonly Action _disposing;
		public QueryContext Context { get; private set; }
		public ResourcesTreatmentType TreatmentType { get; private set; }
		public bool ConnectionOpened { get; private set; }
		public IDataReader DataReader { get; private set; }

		public DataReaderHolder(QueryContext context, IDataReader dataReader, Action disposing)
		{
			_disposing = disposing;
			Context = context;
			DataReader = dataReader;
		}

		public void Dispose()
		{
			_disposing();
		}
		public IEnumerable<IDictionary<string,object>> MapResultToDictionary()
		{
			Func<IDataReader, IDictionary<string, object>> materializer = MapResultToDictionaryMaterializer;
			return MapData<IDictionary<string, object>>(materializer);
		}
		public IEnumerable<T> MapResult<T>(Func<IDataReader, T> materializer)
		{
			return MapData<T>(materializer);
		}
		public IEnumerable<T> MapResult<T>()
		{
			var materializer = _mapMaterializers.GetOrAdd(typeof (T), t =>
				{
					var builder = Context.QueryEnvironment.DelegatesBuilder;
					var entitySchema = Context.QueryEnvironment.SchemaManager.GetSchema(t);
					var info = new EntityMaterializationInformation(t, true);
					return builder.CreateEntityMaterializationDelegate(entitySchema, info);
				});

			return MapData<T>(materializer);
		}

		private IEnumerable<T> MapData<T>(Delegate materializer)
		{
			return new EntityEnumerable<T>(materializer, DataReader, () =>
				{
					if (!DataReader.NextResult())
						Dispose();
				});
		}

		public TReturn MapReturnValue<TReturn>()
		{
			if (Context.PreparationData.ReturnParameterReader == null)
				throw new ObjectSqlException("Return mapping is not set. Use IQueryEnd.Returns<TResult>(object sqlDbType) to set mapping");

			return (TReturn) Context.PreparationData.ReturnParameterReader(Context.Command);
		}

		public IDictionary<string, object> MapResultToDictionaryMaterializer(IDataReader dataReader)
		{
			var result = new Dictionary<string, object>();
			for (int i = 0; i < dataReader.FieldCount; i++ )
			{
				result.Add(dataReader.GetName(i),
						  (dataReader.GetValue(i) is DBNull)? null: dataReader.GetValue(i));
			}
			return result;
		}
	}
}