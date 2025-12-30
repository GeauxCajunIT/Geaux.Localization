# Configuration

The recommended entry point is:

```csharp
builder.Services.AddGeauxLocalization(builder.Configuration, options => { ... });
```

## Options

GeauxLocalizationOptions controls EF behavior, startup migration behavior, and optional seeding behavior.

Common settings:

ConnectionStringName (default: LocalizationConnection)

ConnectionString (optional override)

MigrationsAssembly (recommended for modular solutions)

UseDbContextFactory (default: true)

AutoMigrate (default: true)

AutoSeedLocalizedAttributes (default: false)

SeedOverwriteExisting (default: false)

ModelTypes (types scanned for [Localized])

SupportedCultures (default: en-US)

TenantId (optional, for tenant-specific keys)

DefaultCulture + EnableCultureFallback (localizer behavior)

# Recommended defaults

## Local development (sample/dev)
```
options.AutoMigrate = true;
options.AutoSeedLocalizedAttributes = true;
options.SeedOverwriteExisting = false;
```
## Production
```
options.AutoMigrate = true; // optional; many teams do migrations via deployment pipeline
options.AutoSeedLocalizedAttributes = false; // usually controlled explicitly
```

If you disable AutoMigrate, you must ensure migrations are applied via your deployment process.