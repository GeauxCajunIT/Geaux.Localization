# Geaux.Localization Wiki

Database-backed localization for .NET with tenant awareness, culture fallback, EF Core integration, and an optional admin UI. Use this wiki to quickly wire the library into your application and keep it aligned with the codebase.

## What you get
- Database-backed `IStringLocalizer` with tenant + culture precedence
- EF Core integration with optional startup migrations
- Attribute-based key seeding and repair helpers
- Export/import services for language packs (JSON/CSV/ZIP)
- Admin UI RCL for managing keys, cultures, and translations

## Quick start
1) Install packages
```bash
dotnet add package Geaux.Localization
dotnet add package Microsoft.EntityFrameworkCore.SqlServer  # or Sqlite/Npgsql/MySql
```
2) Add configuration (appsettings.json)
```json
{
  "Localization": {
    "Provider": "SqlServer",
    "ConnectionStringName": "LocalizationConnection",
    "DefaultCulture": "en-US",
    "SupportedCultures": [ "en-US", "fr-FR" ],
    "EnableCultureFallback": true
  },
  "ConnectionStrings": {
    "LocalizationConnection": "Server=.;Database=GeauxLocalization;Trusted_Connection=True;"
  }
}
```
3) Register services
```csharp
builder.Services.AddGeauxLocalization(builder.Configuration, options =>
{
    options.AutoMigrate = true;                 // apply migrations at startup (dev-friendly)
    options.ThrowOnPendingModelChanges = true;  // fail fast if EF model changed without migrations
    options.ModelTypes = [ typeof(Product) ];
    options.SupportedCultures = [ "en-US", "fr-FR" ];
    options.AutoSeedLocalizedAttributes = true; // seed keys from [Localized] attributes
});
```
4) Run the app (or `dotnet ef database update --project src/Geaux.Localization` if you prefer pipeline-driven migrations).

5) Localize with `IStringLocalizer<T>`
```csharp
public class MyService
{
    private readonly IStringLocalizer<MyService> _localizer;
    public MyService(IStringLocalizer<MyService> localizer) => _localizer = localizer;

    public string Greeting() => _localizer["Hello"];
}
```

## Wiki map
- [Getting Started](./Getting-Started.md)
- [Configuration](./Configuration.md)
- [EF Core and Migrations](./EF-Core-and-Migrations.md)
- [Localized Attribute Seeding](./Localized-Attribute-Seeding.md)
- [Tenancy Integration](./Tenancy-Integration.md)
- [Admin UI](./Admin-UI.md)
- [Troubleshooting](./Troubleshooting.md)
- [Changelog](./Changelog.md)
