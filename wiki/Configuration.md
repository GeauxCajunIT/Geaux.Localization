# Configuration

Geaux.Localization is driven by `GeauxLocalizationOptions`. Use `builder.Services.AddGeauxLocalization(builder.Configuration, options => { ... })` to configure them.

## Option reference
| Option | Description | Default |
| --- | --- | --- |
| `ConnectionStringName` | Name in `ConnectionStrings:{name}` to resolve the localization database. Ignored if `ConnectionString` is set. | `LocalizationConnection` |
| `ConnectionString` | Explicit connection string override. | `null` |
| `Provider` | Database provider: `SqlServer`, `Sqlite`, `Npgsql`, `MySql`/`MariaDb`. | `SqlServer` |
| `MigrationsAssembly` | Assembly name containing migrations. | `Geaux.Localization` |
| `UseDbContextFactory` | Register `IDbContextFactory<GeauxLocalizationDbContext>` instead of scoped `DbContext`. | `true` |
| `AutoMigrate` | Run `Database.Migrate()` at startup via hosted service. | `false` |
| `AutoSeedLocalizedAttributes` | Seed `[Localized]` attributes at startup (requires `ModelTypes` + `SupportedCultures`). | `true` |
| `SeedOverwriteExisting` | When seeding, overwrite existing translations if true. | `true` |
| `SupportedCultures` | Cultures used for seeding and culture fallback chain. | `[]` |
| `ModelTypes` | Types to scan for `[Localized]` attributes. | `[]` |
| `DefaultCulture` | Fallback culture when a specific culture value is missing. | `en` |
| `EnableCultureFallback` | Include parent cultures and `DefaultCulture` in lookup chain. | `true` |
| `TenantId` | Optional tenant identifier used for seeding/lookup when provided. | `null` |
| `EnableRetryOnFailure` | Enable SQL Server retry-on-failure. | `true` |
| `ThrowOnPendingModelChanges` | Throw if EF detects pending model changes at runtime. | `false` |

## Recommended baselines
**Local development / samples**
```csharp
options.AutoMigrate = true;
options.AutoSeedLocalizedAttributes = true;
options.SeedOverwriteExisting = true;    // convenient for iterative dev
options.ThrowOnPendingModelChanges = true;
```

**Production**
```csharp
options.AutoMigrate = false;             // apply migrations via your pipeline
options.AutoSeedLocalizedAttributes = false; // seed explicitly if needed
options.SeedOverwriteExisting = false;   // protect existing translations
options.ThrowOnPendingModelChanges = true;
```

## Configuration sources
- **appsettings.json**: place values under a `Localization` section.
- **Code-only**: set options inside the `AddGeauxLocalization` lambda.
- **Environment variable for tooling**: `GEAUX_LOCALIZATION_CONNECTION` is honored by the design-time factory for `dotnet ef` commands.

See [EF Core and Migrations](./EF-Core-and-Migrations.md) for migration guidance and [Localized Attribute Seeding](./Localized-Attribute-Seeding.md) for seeding details.
