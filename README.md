# Geaux.Localization

Tenant-aware, culture-aware, database-backed localization for .NET 8+ and .NET 9 applications.

Geaux.Localization provides a complete end-to-end localization system for multi-tenant and multi-culture
applications. It integrates with Entity Framework Core and ASP.NET Core to deliver database-backed translations that
respect the current tenant and culture for every request.

## Features
- Database-backed `IStringLocalizer` implementation with tenant and culture scoping
- `[Localized]` attribute for annotating model properties that require translation keys
- EF Core `SaveChanges` interceptor that automatically upserts translations for annotated properties
- Seeder utility to create default translations across assemblies
- DI extension for registering context, localizer, and supporting services from configuration
- Support for SQL Server, PostgreSQL, MySQL/MariaDB, and Sqlite providers

## Installation
Add the package reference to your project:

```bash
dotnet add package Geaux.Localization
```

Enable XML documentation so DocFX and IntelliSense can consume the inline XML comments:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```

## Configuration
Add a `Localization` section to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "LocalizationDb": "Server=localhost;Database=GeauxLocalization;Trusted_Connection=True;"
  },
  "Localization": {
    "DefaultCulture": "en-US",
    "SupportedCultures": [ "en-US", "fr-FR" ],
    "EnableCultureFallback": true,
    "ConnectionStringName": "LocalizationDb",
    "Provider": "SqlServer",
    "MigrationsAssembly": "Geaux.Migrations"
  }
}
```

## Usage
Register the localization services in `Program.cs`:

```csharp
using Geaux.Localization.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGeauxLocalization(builder.Configuration);
```

Annotate model properties that need translations:

```csharp
using Geaux.Localization.Attributes;

public class Product
{
    public string TenantId { get; set; } = "default";

    [Localized(
        key: "Product.Name",
        errorMessageKey: "Product.Name.Required",
        displayNameKey: "Product.Name.Label",
        displayMessageKey: "Product.Name.Description")]
    public string Name { get; set; } = string.Empty;
}
```

Translations are stored in the `Translations` table as rows identified by tenant, culture, and key. The
`LocalizationSaveChangesInterceptor` keeps entries in sync during EF Core save operations.

### Provider mapping
| Provider value | EF Core call     | Example                                  |
|----------------|------------------|------------------------------------------|
| SqlServer      | `UseSqlServer`   | `"Provider": "SqlServer"`               |
| PostgreSql     | `UseNpgsql`      | `"Provider": "PostgreSql"`              |
| MySql/MariaDb  | `UseMySql`       | `"Provider": "MySql"`                   |
| Sqlite         | `UseSqlite`      | `"Provider": "Sqlite"`                  |
| LocalDb        | `UseSqlServer`   | `"Provider": "LocalDb"`                 |

## Documentation
Generate DocFX documentation (requires the `docfx` global tool):

```bash
dotnet tool install -g docfx
docfx docfx.json
```

The generated site will be available under the `_site` folder.

## License
MIT License © Brent Lee Rigsby / GeauxCajunIT
