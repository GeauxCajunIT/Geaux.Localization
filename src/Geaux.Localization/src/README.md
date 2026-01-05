# Geaux.Localization

Tenant-aware, culture-aware, database-backed localization for .NET 8+ and .NET 9.

Geaux.Localization provides a complete end-to-end localization system for multi-tenant and multi-culture
applications. It integrates with Entity Framework Core and ASP.NET Core to deliver database-backed translations
that respect the current tenant and culture for every request.

## Features
- Database-backed IStringLocalizer with tenant + culture scoping
- Attribute-based localization via [Localized]
- EF Core SaveChanges interceptor for automatic translation upserts
- Culture fallback + tenant/global precedence
- Seeder utilities for keys, values, and cultures
- Multi-provider support (SQL Server, PostgreSQL, MySQL/MariaDB, Sqlite)
- Export/import tooling (CSV/JSON/ZIP)

## Installation
``` text
dotnet add package Geaux.Localization
```

## Quick Start

```csharp
builder.Services.AddGeauxLocalization(builder.Configuration);
```
Add to appsettings.json:

```json
{
  "Localization": {
    "DefaultCulture": "en-US",
    "SupportedCultures": [ "en-US", "fr-FR" ],
    "EnableCultureFallback": true,
    "ConnectionStringName": "LocalizationDb",
    "Provider": "SqlServer"
  }
}
```

## License
MIT © Brent Lee Rigsby / GeauxCajunIT
