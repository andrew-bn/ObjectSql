using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using ObjectSql.Core.Bo;
using ObjectSql.QueryInterfaces;

namespace ObjectSql.Core
{
	internal class QueryDataReader : IQueryDataReader
	{
		private static readonly ConcurrentDictionary<Type, Delegate> _mapMaterializers = new ConcurrentDictionary<Type, Delegate>();

		private readonly Action _disposing;
		public QueryContext Context { get; private set; }
		public ResourcesTreatmentType TreatmentType { get; private set; }
		public bool ConnectionOpened { get; private set; }
		public IDataReader DataReader { get; private set; }

		public QueryDataReader(QueryContext context, IDataReader dataReader, Action disposing)
		{
			_disposing = disposing;
			Context = context;
			DataReader = dataReader;
		}

		public void Dispose()
		{
			_disposing();
		}

		public System.Collections.Generic.IEnumerable<T> MapResult<T>()
		{
			var materializer = _mapMaterializers.GetOrAdd(typeof (T), t =>
				{
					var builder = Context.QueryBuilder.DelegatesBuilder;
					var entitySchema = Context.SchemaManager.GetSchema(t);
					var info = new EntityMaterializationInformation(t, true);
					return builder.CreateEntityMaterializationDelegate(entitySchema, info);
				});

			return new EntityEnumerable<T>(materializer, DataReader, () =>
			{
				if (!DataReader.NextResult())
					Dispose();
			});
		}
	}
}