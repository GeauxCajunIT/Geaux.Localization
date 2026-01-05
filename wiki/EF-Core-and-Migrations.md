# EF Core and Migrations

Geaux.Localization ships with `GeauxLocalizationDbContext` and migrations for the localization schema.

## Registering the DbContext
- `UseDbContextFactory` (default `true`) registers `IDbContextFactory<GeauxLocalizationDbContext>` to keep the DbContext short-lived and thread-safe for localization lookups.
- Set `UseDbContextFactory = false` if you prefer a scoped DbContext.
- `Provider` controls which EF provider is used (`SqlServer`, `Sqlite`, `Npgsql`, `MySql`/`MariaDb`).

## Applying migrations
**Automated (dev-friendly)**
```csharp
options.AutoMigrate = true;
```
A hosted service runs `db.Database.Migrate()` at startup before seeding.

**Pipeline/CI (recommended for production)**
```bash
dotnet ef database update --project src/Geaux.Localization
```
- The design-time factory honors `GEAUX_LOCALIZATION_CONNECTION` for `dotnet ef` commands.
- Override `MigrationsAssembly` when you copy migrations into another assembly.

## Guardrails
- `ThrowOnPendingModelChanges = true` makes EF throw when the runtime model differs from applied migrations.
- `EnableRetryOnFailure = true` (default) enables SQL Server resiliency options.

## Connection strings
Order of precedence:
1. `options.ConnectionString` (explicit)
2. `ConnectionStrings:{ConnectionStringName}` from configuration (defaults to `LocalizationConnection`)
3. Throws an exception if neither is provided

## DbContext usage inside the library
- Localization lookups and seeding use the factory to create transient DbContexts.
- Tenant-aware lookups filter translations by the current tenant (or global) and prefer tenant-specific values when they exist.
