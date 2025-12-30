# Getting Started

Geaux.Localization provides a database-backed `IStringLocalizer` implementation using EF Core.

## 1. Add package references


Core:


```bash

dotnet add package Geaux.Localization
```

EF provider (example SQL Server):

  
Copy code

```bash
 dotnet add package Microsoft.EntityFrameworkCore.SqlServer	
```

## 2. Configure connection string

appsettings.json:



json

Copy code
 ```
{

&nbsp; "ConnectionStrings": {

&nbsp;   "LocalizationConnection": "Server=(localdb)\\\\MSSQLLocalDB;Database=GeauxLocalization;Trusted\_Connection=True;"

&nbsp; }

}
 ```

## 3. Register services

Program.cs:



csharp

Copy code
 ```
builder.Services.AddGeauxLocalization(builder.Configuration, options =>

{

&nbsp;   options.ConnectionStringName = "LocalizationConnection";

&nbsp;   options.MigrationsAssembly = "Geaux.Localization";

&nbsp;   options.AutoMigrate = true;

&nbsp;   options.UseDbContextFactory = true;

});
 ```

## 4\. Use localizers

Use IStringLocalizer<T>:
csharp

Copy code
 ```
public class MyService

{

&nbsp;   private readonly IStringLocalizer<MyService> \_L;



&nbsp;   public MyService(IStringLocalizer<MyService> L) => \_L = L;



&nbsp;   public string Greeting() => \_L\["Hello"];

}
 ```
