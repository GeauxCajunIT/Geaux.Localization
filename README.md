   # Geaux.Localization

Database‑backed localization for .NET with multi‑tenant support, EF Core integration, culture fallback, and export/import tooling.

<p align="left">
<img src="https://img.shields.io/nuget/v/Geaux.Localization?color=4c1&label=NuGet%20Version" />
<img src="https://img.shields.io/nuget/dt/Geaux.Localization?color=blue&label=Downloads" />
<img src="https://img.shields.io/github/actions/workflow/status/GeauxCajunIT/Geaux.Localization/build.yml?label=Build" />
<img src="https://img.shields.io/github/license/GeauxCajunIT/Geaux.Localization?color=yellow" />
<img src="https://img.shields.io/badge/.NET-9.0-blueviolet" />
<img src="https://img.shields.io/badge/EF%20Core-9.0-512BD4" />
</p>

Designed for:

- Multi-tenant systems
- EF Core
- Clean Architecture
- NuGet distribution
- Admin UI via a separate RCL (`Geaux.Localization.Admin`)

---

## ✨ Features

- Database-backed translations (EF Core)
- Tenant-aware localization (`TenantId`)
- Culture fallback support
- Attribute-based model localization via `[Localized]`
- EF Core `SaveChanges` interceptor for automatic key creation
- Seeding pipeline:
  - Model attribute seeding
  - Key and culture seeding
  - Repair of missing `(Key × Culture)` combinations
- Export/import services for language packs (JSON/CSV/ZIP)
- Fully self-contained core package (no ASP.NET framework reference)

---

## 📦 Installation

```bash
dotnet add package Geaux.Localization
```

Admin UI (optional):

```bash
dotnet add package Geaux.Localization.Admin
```

## ⚙️ Configuration

appsettings.json:
``` json
{
  "Localization": {
    "Provider": "SqlServer",
    "ConnectionStringName": "LocalizationDb",
    "DefaultCulture": "en-US",
    "TenantId": "tenant-1"
  }
}
```

## 🔧 Service Registration

Configuration-based
```csharp
builder.Services.AddGeauxLocalization(
    builder.Configuration.GetSection("Localization"));
```

Code-based

```csharp
builder.Services.AddGeauxLocalization(options =>
{
    options.Provider = "Sqlite";
    options.ConnectionString = "Data Source=localization.db";
    options.DefaultCulture = "en-US";
    options.TenantId = "tenant-1";
});
```

## 🗄️ DbContext & Migrations
GeauxLocalizationDbContext is the core EF Core context.

Design-time connection string:

``` bash
set GEAUX_LOCALIZATION_CONNECTION=Server=.;Database=LocalizationDb;Trusted_Connection=True;
```

Apply migrations as usual:

``` bash
dotnet ef database update --project src/Geaux.Localization
```

## 🏷️ Attribute-Based Localization
``` csharp
using Geaux.Localization.Attributes;

public class Product
{
    [Localized(
        "Product.Name",
        DisplayNameKey = "Product.Name.Display",
        ErrorMessageKey = "Product.Name.Required")]
    public string Name { get; set; } = string.Empty;
}
```

At startup, you can scan and seed keys:

``` csharp
await app.Services.InitializeGeauxLocalizationDatabaseAsync();
```

This uses:

- LocalizationKeyScanner

- ModelAttributeSeeder

- CultureSeeder

- KeySeeder

## 🌍 Using IStringLocalizer
``` csharp
public class MyService
{
    private readonly IStringLocalizer<MyService> _localizer;

    public MyService(IStringLocalizer<MyService> localizer)
    {
        _localizer = localizer;
    }

    public string GetGreeting()
    {
        return _localizer["Hello"];
    }
}
```

Tenant behavior:

- TenantId = null → global translation

- TenantId != null → tenant-scoped translation

Global and tenant translations are stored and enforced separately.

## 📤 Export & 📥 Import (Language Packs)
The core library exposes:

- LocalizationExportService

- LocalizationImportService

The admin RCL (Geaux.Localization.Admin) wires these into:

- JSON export/import

- CSV export/import

- ZIP export/import (all cultures)

See the Admin documentation in the GitHub wiki for routes and UI usage.

## 🧪 Testing
The test project includes coverage for:

- Culture resolution

- Tenant scoping

- Seeding behavior

- EF Core integration

- Export/import behavior

Run tests:

``` bash
dotnet test
```

## 📚 Documentation & Wiki
- Repository: https://github.com/GeauxCajunIT/Geaux.Localization

- Wiki: https://github.com/GeauxCajunIT/Geaux.Localization/wiki

Wiki includes:

- Getting Started

- Configuration

- Admin UI

- Export/Import

- Seeding & Maintenance

- Multi-tenant behavior

## 🧩 Project Structure (Core)
```text
src/Geaux.Localization/
  Attributes/
  Config/
  Contexts/
  EFCore/
    EntityConfiguration/
    Interceptors/
    Seeding/
    Startup/
  Extensions/
  Interfaces/
  Migrations/
  Models/
  Resources/
  Scanning/
  Services/
    Engine/
    ExportImport/
    Maintenance/
    Startup/
 ```

Admin UI lives in Geaux.Localization.Admin as a separate RCL.

## 🤝 Contributing
Contributions are welcome!

- Fork the repo

- Create a feature branch

- Add tests where appropriate

- Open a pull request

See the CONTRIBUTING page for guidelines.

## 📜 License
MIT
