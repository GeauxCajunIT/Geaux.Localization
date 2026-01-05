# Admin UI

The `Geaux.Localization.Admin` Razor Class Library provides a management dashboard and REST endpoints for cultures, keys, translations, and language packs.

## Features
- Culture, key, and translation management with tenant awareness
- Inline editing with search/filter by key prefix, culture, and tenant
- Export/import (JSON, CSV, missing-only CSV/JSON, ZIP bundles)
- Maintenance tools for repairing missing values
- Built with MudBlazor (theme-aware)

## Setup
1) Install packages
```bash
dotnet add package Geaux.Localization.Admin
dotnet add package Geaux.Localization         # core dependency
```
2) Register services and endpoints
```csharp
builder.Services.AddGeauxLocalization(builder.Configuration);
builder.Services.AddGeauxLocalizationAdmin();

var app = builder.Build();
app.MapGeauxLocalizationAdminEndpoints();
```
3) Use the UI
- Browse to the admin endpoint exposed by your host application (the package provides endpoint wiring; surface it as appropriate for your app).

## API endpoints
Example endpoint shapes (adapt to your chosen base path):
```
GET  /admin/localization/api/cultures
POST /admin/localization/api/keys
GET  /admin/localization/api/language-pack/{culture}
POST /admin/localization/api/maintenance/repair
```
These endpoints power the UI and can be used programmatically for automation.

## Export/Import services
The admin package wires the core `LocalizationExportService` and `LocalizationImportService` to support:
- JSON export/import
- CSV export/import
- ZIP bundles containing all cultures

See [Configuration](./Configuration.md) for option defaults and [Tenancy Integration](./Tenancy-Integration.md) for tenant considerations.
