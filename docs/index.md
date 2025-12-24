# Geaux.Localization

Database-backed localization for .NET via `IStringLocalizer`, with seeding and an EF Core SaveChanges interceptor.

## Quick start

### Install
```bash
dotnet add package Geaux.Localization
```

### Register services
```csharp
// appsettings.json section: "Localization"
builder.Services.AddGeauxLocalization(builder.Configuration.GetSection("Localization"));
```

### Typical configuration (appsettings.json)
```json
{
  "Localization": {
    "Provider": "Sqlite",
    "ConnectionString": "Data Source=localization.db",
    "DefaultCulture": "en-US",
    "TenantId": null
  }
}
```

## API Reference

See the **API** section in the navigation for generated reference docs.
