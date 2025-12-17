---
title: Geaux.Localization
uid: Geaux.Localization.index
---

# Geaux.Localization

Geaux.Localization is a tenant-aware, culture-aware localization library for ASP.NET Core applications. It delivers
translations from a database using EF Core and respects the active tenant and culture on every request.

## Highlights
- Database-backed `IStringLocalizer` scoped to tenant and culture
- `[Localized]` attribute to declare translation keys on model properties
- EF Core `SaveChanges` interceptor that automatically upserts translations
- Seeder utility for creating default translations across assemblies
- Supports SQL Server, PostgreSQL, MySQL/MariaDB, and Sqlite providers

## Getting started
1. Add the NuGet package:
   ```bash
   dotnet add package Geaux.Localization
   ```
2. Configure localization in `appsettings.json` and register services in `Program.cs` with
   `services.AddGeauxLocalization(configuration);`
3. Decorate model properties with `[Localized]` to ensure translation keys are created and kept in sync.

## Documentation build
Generate the API and conceptual documentation with DocFX:

```bash
docfx docfx.json
```

The generated static site will be available in the `_site` folder.
