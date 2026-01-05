# Troubleshooting

## "Connection string 'LocalizationConnection' was not found"
Cause: neither `ConnectionStrings:{ConnectionStringName}` nor `options.ConnectionString` is set.

Fix:
- Add a connection string to configuration (default name: `LocalizationConnection`), or
- Set `options.ConnectionString` explicitly, or
- For tooling, set `GEAUX_LOCALIZATION_CONNECTION` when running `dotnet ef`.

## "Invalid object name 'LocalizationKeys'"
Cause: migrations not applied or wrong database.

Fix:
- Verify the connection string and target database.
- Run `dotnet ef database update --project src/Geaux.Localization` or enable `options.AutoMigrate`.
- Ensure `MigrationsAssembly` matches where your migrations live.

## "Cannot resolve DbContextFactory from root provider"
Cause: resolving EF services from the root without a scope.

Fix:
```csharp
using var scope = app.Services.CreateScope();
var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<GeauxLocalizationDbContext>>();
```

## Translations not appearing
- Confirm the requested culture exists in `SupportedCultures`.
- Ensure the keys exist (`LocalizationKeys`) and have values for the culture.
- If `[Localized]` attributes changed, re-run seeding with `AutoSeedLocalizedAttributes` and `ModelTypes` configured (or call `ModelAttributeSeeder.SeedAsync` manually).

## Tenant-specific values not loading
- Make sure a tenant identifier is passed (via `options.TenantId` or `localizer.WithTenant(...)`).
- Remember that global translations are used as a fallback when no tenant value exists.
