ObjectSql
========
ObjectSql is a lightweight component that simplifies Database access.

* Supports MsSql database
* Object mappers for result sets
* StoredProcedure calling
* Bulk Update/Delete/Insert
* Sql support

--------
ObjectSql is an extension for *IDbConnection*, *IDbCommand*
## Schema generation
ObjectSql.SqlServer project contains a tt file with name ConnStrHere.SqlServer.tt.
In order to generate schema do the following:
* Add reference to ObjectSql.dll and ObjectSql.SqlServer.dll to your project and recompile
	![Add-Reference ObjectSql](https://github.com/andrew-bn/ObjectSql/blob/master/images/ObjSql1.png)
* Copy ConnStrHere.SqlServer.tt into your project into some directory with the name of Database
* Add App.config or Web.config to your project and define ConnectionString setting
* Rename ConnStrHere.SqlServer.tt to <Connection string setting name from App.Config>.SqlServer.tt
	![Configure ObjectSql](https://github.com/andrew-bn/ObjectSql/blob/master/images/ObjSql2.png)
* Right-click on tt file and click "Run custom tool"
* If connection with DB was established then you'll get C# generated schema 
	* db shemas are generated as C# namespaces
	* tables are generated as partial C# classes
		![Schema1 ObjectSql](https://github.com/andrew-bn/ObjectSql/blob/master/images/ObjSql3.png)
	* stored procedures are generated as methods of abstract class with name <ConnectionStringName>Procedures

## Access to ObjectSql infrastructure
Access from SqlConnection
``` CSharp
  using (var c = new SqlConnection(_connectionString))
	{
			var result = c.ObjectSql().From<Product>()
			             .Select(p => p)
			             .ExecuteQuery().ToArray();
	}
```	
Access from SqlCommand
``` CSharp
	using (var c = new SqlConnection(_connectionString))
	{
		using (var cmd = c.CreateCommand())
					var result = cmd.ObjectSql().From<Product>()
					               .Select(p => p)
					               .ExecuteQuery().ToArray();
	}
``` 
Access from ObjectSqlManager
``` CSharp
	var manager = new ObjectSqlManager<SqlConnection>(_connectionString);
	var result = manager.Query().From<Product>().Select(p => p).ExecuteQuery();
```
