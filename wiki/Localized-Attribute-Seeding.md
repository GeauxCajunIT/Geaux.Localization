# Localized Attribute Seeding

Geaux.Localization can scan your models for `[Localized]` attributes and seed keys/values automatically.

## Enabling startup seeding
```csharp
builder.Services.AddGeauxLocalization(builder.Configuration, options =>
{
    options.AutoSeedLocalizedAttributes = true; // default: true
    options.ModelTypes = [ typeof(Product), typeof(Order) ];
    options.SupportedCultures = [ "en-US", "es-ES" ];
    options.SeedOverwriteExisting = false;     // keep existing translations
});
```
Startup seeding runs after migrations when `AutoSeedLocalizedAttributes` is true **and** both `ModelTypes` and `SupportedCultures` are populated.

## What gets created
- Keys are derived from `[Localized]` properties (`Key`, `DisplayNameKey`, `DisplayMessageKey`, `ErrorMessageKey`).
- For each culture in `SupportedCultures` (or the attribute’s `Culture` override), a `LocalizationValue` is inserted.
- Default values are based on property names or the tail segment of the key (e.g., `.Name`, `.Display`).
- `SeedOverwriteExisting = true` will replace existing values; otherwise only missing values are added.

## Tenant-aware seeding
- The hosted service seeds **global** scope (no tenant) by default.
- To seed per-tenant keys, call the seeder manually:
  ```csharp
  await ModelAttributeSeeder.SeedAsync(
      factory,
      modelTypes: [ typeof(Product) ],
      supportedCultures: [ "en-US", "es-ES" ],
      tenantId: "tenant-123",
      overwrite: false,
      ct: cancellationToken);
  ```

## When to re-run seeding
- After adding/updating `[Localized]` attributes.
- After adding new supported cultures.
- After introducing a new tenant that needs base translations.
