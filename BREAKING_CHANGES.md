# Breaking Changes

This document tracks intentional breaking changes across major and minor versions
of **Geaux.Localization**.

---

## ❗ v2.0.0

### Package & Architecture

- **Refactored project structure** for clearer separation of concerns:
  - Core library: `Geaux.Localization`
  - Admin UI: `Geaux.Localization.Admin` (RCL)
  - Samples: `Geaux.Localization.AspireSample.*`
- Core package is now **UI-agnostic**:
  - No MudBlazor, Razor components, or controllers in the core library.
  - All admin APIs and UI moved to `Geaux.Localization.Admin`.

### EF Core & DbContext

- Standardized DbContext name:
  - `GeauxLocalizationDbContext` is now the canonical context name.
- EF Core configuration moved into:
  - `EFCore/EntityConfiguration`
  - `EFCore/Interceptors`
  - `EFCore/Seeding`
  - `Startup/DesignTimeLocalizationDbContextFactory.cs`

> **Action required:**  
> Update any references to older DbContext names or configuration locations.

### Seeding & Maintenance

- New **seeding and maintenance pipeline**:
  - `CultureSeeder`
  - `KeySeeder`
  - `ModelAttributeSeeder`
  - `LocalizationMaintenanceService`
  - `LocalizationStartupHostedService`
  - `LocalizationRepairHostedService`
- Behavior change:
  - All `(Key × Culture)` combinations can now be pre-seeded.
  - Missing values are treated as empty strings rather than missing rows.

> **Action required:**  
> If you previously relied on values only being created when edited, review your
export/import and UI assumptions.

### Attribute & Scanning

- `LocalizedAttribute` remains the primary attribute, but:
  - `ModelAttributeSeeder` and `LocalizationKeyScanner` now drive key discovery.
- Attribute-based seeding is now **idempotent** and **model-driven**.

---

## ❗ v1.3.0

### Removed

- `Microsoft.AspNetCore.App` framework reference.
- Duplicate `Options/GeauxLocalizationOptions.cs`.

### Database Schema

- `Translation` (now `LocalizationValue`) includes:
  - `TenantId` (nullable).
- Unique index changed from:
  - `(Culture, Key)`
  - ➜ `(TenantId, Culture, Key)`.

> **Migration required.**

### Attribute Contract

- `LocalizedAttribute` properties are now writable:
  - `DisplayNameKey`
  - `ErrorMessageKey`
  - `DisplayMessageKey`.

### Dependency Injection

- New overloads:
  - `services.AddGeauxLocalization(Action<GeauxLocalizationOptions> configure)`
  - `services.AddGeauxLocalization(IConfigurationSection section)`

