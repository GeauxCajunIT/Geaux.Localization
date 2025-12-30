# Localized Attribute Seeding

Geaux.Localization can generate localization keys automatically by scanning your model types for `[Localized]` attributes.

## Enable auto-seeding

```csharp
builder.Services.AddGeauxLocalization(builder.Configuration, options =>
{
    options.AutoSeedLocalizedAttributes = true;
    options.ModelTypes = new[] { typeof(Product), typeof(Order) };
    options.SupportedCultures = new[] { "en-US", "es-ES" };
});
Overwrite behavior
SeedOverwriteExisting = false (default) will only add missing keys/translations.

SeedOverwriteExisting = true will update existing values (use carefully).

Tenant-aware seeding
To seed keys for a tenant:

csharp
Copy code
options.TenantId = "tenant-abc";
This creates keys in the tenant scope (depending on your schema rules).

When seeding runs
Seeding runs at startup after migrations when enabled.

If you do not want startup seeding, disable AutoSeedLocalizedAttributes and call your seeder explicitly from an admin tool or migration step.

yaml
Copy code
