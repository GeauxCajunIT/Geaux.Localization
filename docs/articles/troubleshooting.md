# Troubleshooting

## “connection string was not found”
Geaux.Localization requires either:
- `options.ConnectionString`, or
- a configured connection string named `options.ConnectionStringName`.

Example:

```json
{
  "ConnectionStrings": {
    "LocalizationConnection": "..."
  }
}
“Invalid object name 'LocalizationKeys'”
Your DB schema is missing tables. Common causes:

migrations not applied

wrong connection string

wrong migrations assembly

Fix:

verify connection string points to expected DB

set options.MigrationsAssembly

ensure AutoMigrate = true (or apply migrations via pipeline)

“Cannot resolve DbContextFactory … from root provider”
This typically occurs when resolving scoped EF services from the root provider. Fix by resolving inside a scope:

csharp
Copy code
using var scope = app.Services.CreateScope();
var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<GeauxLocalizationDbContext>>();
Model changes not appearing
If your model changes require schema changes:

add a new migration in your migrations project

apply migrations (AutoMigrate or pipeline)

If only [Localized] attributes changed:

ensure AutoSeedLocalizedAttributes = true

ensure the changed types are included in ModelTypes