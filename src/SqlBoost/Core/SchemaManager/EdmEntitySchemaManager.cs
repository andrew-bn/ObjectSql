using SqlBoost.Core.Bo.EntitySchema;
using SqlBoost.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SqlBoost.Core.SchemaManager
{
	internal class EdmEntitySchemaManager<TTypeEnum> : EntitySchemaManager<TTypeEnum>
		where TTypeEnum: struct
	{
		private readonly XDocument _csdlDocument;
		private readonly XDocument _ssdlDocument;
		private readonly XDocument _mslDocument;

		private readonly string _csdlNamespace;
		private readonly string _ssdlNamespace;

		public EdmEntitySchemaManager(string metadata)
		{
			var resources = metadata.Split('|').Select(x => x.Replace(@"res://*/", "")).ToArray();

			var csdlResName = resources.SingleOrDefault(x => x.EndsWith("csdl"));
			var ssdlResName = resources.SingleOrDefault(x => x.EndsWith("ssdl"));
			var mslResName = resources.SingleOrDefault(x => x.EndsWith("msl"));

			if(csdlResName == null || ssdlResName == null || mslResName==null)
				throw new SqlBoostException("Invalid metadata description. Metadata not found");

			var asms = AppDomain.CurrentDomain.GetAssemblies();
			foreach (var asm in asms)
			{
				if (asm.IsDynamic) continue;

				var names = asm.GetManifestResourceNames();
				if (names.Contains(csdlResName) &&
					names.Contains(ssdlResName) &&
					names.Contains(mslResName))
				{
					using (var csdlStream = asm.GetManifestResourceStream(csdlResName))
						_csdlDocument = XDocument.Load(csdlStream);
					using (var ssdlStream = asm.GetManifestResourceStream(ssdlResName))
						_ssdlDocument = XDocument.Load(ssdlStream);
					using (var mslStream = asm.GetManifestResourceStream(mslResName))
						_mslDocument = XDocument.Load(mslStream);

					_csdlNamespace = _csdlDocument.Descendants().First(d => d.Name.LocalName == "Schema").Attribute("Namespace").Value + ".";
					_ssdlNamespace = _ssdlDocument.Descendants().First(d => d.Name.LocalName == "Schema").Attribute("Namespace").Value + ".";

					break;
				}
			}
			if (_csdlDocument == null || _csdlNamespace == null ||
				_mslDocument == null || _ssdlDocument == null ||
				_ssdlNamespace == null)
				throw new SqlBoostException("Invalid metadata description. Metadata not found");
		}
		protected override EntitySchema CreateSchema(Type entity)
		{
			EntitySchema result;
			if (!TryCreateSchemaForEntityType(entity, out result)&&
				!TryCreateSchemaForComplexType(entity, out result))
				result = base.CreateSchema(entity);
			return result;
		}
		private bool TryCreateSchemaForEntityType(Type entity, out EntitySchema schema)
		{
			schema = null;

			var entityTypeName = _csdlNamespace + entity.Name;
			var entityDeclarationFound =
				(from item in _csdlDocument.Descendants()
				 where item.Name.LocalName == "EntitySet" &&
					   item.Attribute("EntityType").Value == entityTypeName
				 select item).Any();

			if (!entityDeclarationFound) return false;

			var entityToStoreMapping = (from item in _mslDocument.Descendants()
										where item.Name.LocalName == "EntityTypeMapping"
										   && item.Attribute("TypeName").Value == entityTypeName
										select item).FirstOrDefault();

			if (entityToStoreMapping == null) return false;

			var storageSetName = (from item in entityToStoreMapping.Descendants()
								  where item.Name.LocalName == "MappingFragment"
								  select item.Attribute("StoreEntitySet").Value).FirstOrDefault();

			if (storageSetName == null) return false;

			var fieldsMap = (from item in entityToStoreMapping.Descendants()
							 where item.Name.LocalName == "ScalarProperty"
							 select new
									 {
										 EntityField = item.Attribute("Name").Value,
										 StorageField = item.Attribute("ColumnName").Value
									 }).ToArray();

			var storageName = (from item in _ssdlDocument.Descendants()
							   where item.Name.LocalName == "EntitySet"
								   && item.Attribute("Name").Value == storageSetName
							   select
								new
								{
									EntityType = item.Attribute("EntityType").Value.Replace(_ssdlNamespace, ""),
									StorageName = new StorageName(item.Attribute("Table") != null ? item.Attribute("Table").Value : item.Attribute("EntityType").Value.Replace(_ssdlNamespace, ""),
																	item.Attribute("Schema") != null ? item.Attribute("Schema").Value : "")
								}
							  ).FirstOrDefault();

			if (storageName == null) return false;

			var storageTypeDefinition = (from item in _ssdlDocument.Descendants()
										 where item.Name.LocalName == "EntityType"
											 && item.Attribute("Name").Value == storageName.EntityType
										 select item)
										 .FirstOrDefault();

			if (storageTypeDefinition == null) return false;

			var storageTypeFields = (from item in storageTypeDefinition.Descendants()
									 where item.Name.LocalName == "Property"
									 select new
											 {
												 Name = item.Attribute("Name").Value,
												 TypeName = item.Attribute("Type").Value
											 }).ToArray();

			var map = (from fldMap in fieldsMap
					   join storageField in storageTypeFields on fldMap.StorageField equals storageField.Name
					   select new { fldMap, storageField })
					  .ToDictionary(v => v.fldMap.EntityField,
									v => new StorageField(v.storageField.Name,
										ParseDbType(v.storageField.TypeName)));

			schema = new EntitySchema(entity, storageName.StorageName, map);
			return true;
		}
		private bool TryCreateSchemaForComplexType(Type entity, out EntitySchema schema)
		{
			schema = null;

			var entityTypeName = _csdlNamespace + entity.Name;
			var entityToStoreMapping = (from item in _mslDocument.Descendants()
										where item.Name.LocalName == "ComplexTypeMapping"
										   && item.Attribute("TypeName").Value == entityTypeName
										select item).FirstOrDefault();

			if (entityToStoreMapping == null) return false;
			var fieldsMap = (from item in entityToStoreMapping.Descendants()
							 where item.Name.LocalName == "ScalarProperty"
							 select new
							 {
								 EntityProperty = item.Attribute("Name").Value,
								 StorageField = item.Attribute("ColumnName").Value
							 }).ToArray();
			var map = fieldsMap.ToDictionary(v => v.EntityProperty, v => new StorageField(v.StorageField, null));

			schema = new EntitySchema(entity, null, map);
			return true;
		}

		protected override FuncSchema CreateFuncSchema(System.Reflection.MethodInfo method)
		{
			FuncSchema result;
			if (!TryCreateFuncSchema(method,out result))
				result = base.CreateFuncSchema(method);
			return result;
		}

		private bool TryCreateFuncSchema(System.Reflection.MethodInfo method, out FuncSchema result)
		{
			result = null;
			var funcName = method.Name;
			var storageFuncName = (from item in _mslDocument.Descendants()
							   where item.Name.LocalName == "FunctionImportMapping"
								   && item.Attribute("FunctionImportName").Value == funcName
								   select item.Attribute("FunctionName").Value).FirstOrDefault();

			if (String.IsNullOrEmpty(storageFuncName))
				return false;
			if (storageFuncName.StartsWith(_ssdlNamespace))
				storageFuncName = storageFuncName.Substring(_ssdlNamespace.Length);

			var storageFuncDescription = (from item in _ssdlDocument.Descendants()
										  where item.Name.LocalName == "Function"
											  && item.Attribute("Name").Value == storageFuncName
										  select item).FirstOrDefault();

			if (storageFuncDescription == null)
				return false;

			var storeFuncName = storageFuncDescription.Attribute("StoreFunctionName");
			var storageName = new StorageName(storeFuncName != null ? storeFuncName.Value : storageFuncName,
											  storageFuncDescription.Attribute("Schema").Value);

			var parameters = (from item in storageFuncDescription.Descendants()
							  where item.Name.LocalName == "Parameter"
							  select new
							  {
								  Name = item.Attribute("Name").Value,
								  Type = item.Attribute("Type").Value
							  }).ToArray();

			var funcParams = new List<FuncParameter>();
			var methodParams = method.GetParameters().Select(p => p.Name.ToUpper()).ToArray();

			foreach (var p in parameters)
			{
				var paramIndex = Array.IndexOf(methodParams, p.Name.ToUpper());
				var funcStorageName = new StorageField(p.Name, ParseDbType(p.Type));
				funcParams.Add(new FuncParameter(p.Name, paramIndex, funcStorageName));
			}

			result = new FuncSchema(storageName, funcParams.ToArray());

			return true;
		}
	}
}
