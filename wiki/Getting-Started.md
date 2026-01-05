# Getting Started

Follow these steps to get Geaux.Localization running in a new app.

## 1) Install packages
- Core library:
  ```bash
  dotnet add package Geaux.Localization
  ```
- EF Core provider (choose the one for your database):
  ```bash
  dotnet add package Microsoft.EntityFrameworkCore.SqlServer
  # or: Microsoft.EntityFrameworkCore.Sqlite / Npgsql.EntityFrameworkCore.PostgreSQL / Pomelo.EntityFrameworkCore.MySql
  ```

## 2) Configure connection + localization settings
`appsettings.json`
```json
{
  "Localization": {
    "Provider": "SqlServer",
    "ConnectionStringName": "LocalizationConnection",
    "DefaultCulture": "en-US",
    "SupportedCultures": [ "en-US", "es-ES" ],
    "EnableCultureFallback": true
  },
  "ConnectionStrings": {
    "LocalizationConnection": "Server=.;Database=GeauxLocalization;Trusted_Connection=True;"
  }
}
```

## 3) Register services
```csharp
builder.Services.AddGeauxLocalization(builder.Configuration, options =>
{
    options.AutoMigrate = true;                 // apply migrations at startup (handy for dev)
    options.UseDbContextFactory = true;         // default; keeps DbContexts factory-based
    options.ThrowOnPendingModelChanges = true;  // optional safety for CI/dev
});
```

## 4) Apply migrations
- Set a connection string (or `GEAUX_LOCALIZATION_CONNECTION` for design-time tooling).
- Run `dotnet ef database update --project src/Geaux.Localization` **or** rely on `options.AutoMigrate = true` to run at app startup.

## 5) Seed keys (optional but recommended)
Enable attribute scanning when you have `[Localized]` attributes:
```csharp
options.AutoSeedLocalizedAttributes = true;
options.ModelTypes = [ typeof(Product), typeof(Order) ];
options.SupportedCultures = [ "en-US", "es-ES" ];
options.SeedOverwriteExisting = false; // keep existing translations when re-seeding
```

## 6) Use the localizer
```csharp
public class OrdersService
{
    private readonly IStringLocalizer<OrdersService> _L;
    public OrdersService(IStringLocalizer<OrdersService> localizer) => _L = localizer;

    public string Greeting() => _L["Hello"];
}
```

Next steps: review [Configuration](./Configuration.md) for option defaults and [Tenancy Integration](./Tenancy-Integration.md) for multi-tenant behavior.
