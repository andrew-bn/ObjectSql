#ObjectSql
=======

[Wiki Home](https://github.com/andrew-bn/ObjectSql/wiki)


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
