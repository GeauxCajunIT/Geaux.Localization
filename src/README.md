# Geaux.Localization

Database-backed localization provider for .NET using `IStringLocalizer`.

Designed for:
- Multi-tenant systems
- EF Core
- Clean Architecture
- NuGet distribution

---

## ✨ Features

- Database-backed translations
- Tenant-aware localization
- Culture fallback support
- Attribute-based model localization
- EF Core SaveChanges interceptor
- Seeder for missing localization keys
- Fully self-contained NuGet package

---

## 📦 Installation

```bash
dotnet add package Geaux.Localization
```

## ⚙️ Configuration
```
appsettings.json
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
services.AddGeauxLocalization(configuration.GetSection("Localization"));

Code-based
services.AddGeauxLocalization(options =>
{
    options.Provider = "Sqlite";
    options.ConnectionString = "Data Source=localization.db";
    options.TenantId = "tenant-1";
});

## 🏷️ Attribute Usage
```
public class Product
{
    [Localized(
        "Product.Name",
        DisplayNameKey = "Product.Name.Display",
        ErrorMessageKey = "Product.Name.Required")]
    public string Name { get; set; }
}
```

## 🌍 Localization Lookup
```
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

## Database Initialization

Geaux.Localization does not automatically create or migrate databases.
You must initialize the schema at application startup.

### Recommended pattern

```csharp
await app.Services.InitializeGeauxLocalizationDatabaseAsync();
```

 ## Attribute-Based Seeding

Model properties decorated with `[Localized]` can be scanned at startup
to automatically create translation keys in the database.

This is useful for:
- Display labels
- Validation messages
- System-owned UI text

Application code can seed these keys safely without overwriting
user-edited translations.

 ### Tenant Behavior

- `TenantId = null` → global translation
- `TenantId != null` → tenant-scoped translation

Global and tenant translations are stored and enforced separately.
## 🧪 Testing

Includes tests for:

Culture resolution

Tenant scoping

Seeding behavior

EF Core integration


## 🛠️ Migrations

Design-time connection string:
 ```
set GEAUX_LOCALIZATION_CONNECTION=Server=.;Database=LocalizationDb;Trusted_Connection=True;
 ```

## 📜 License

MIT