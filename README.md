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
