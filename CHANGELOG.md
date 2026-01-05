  # Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/)
and this project adheres to [Semantic Versioning](https://semver.org/).

---

## [2.0.0] – 2025-01-XX

### Added

- Clean, modular project structure:
  - `Geaux.Localization` (core engine)
  - `Geaux.Localization.Admin` (admin RCL)
  - `Geaux.Localization.AspireSample.*` (samples)
- New services:
  - `LocalizationMaintenanceService`
  - `LocalizationCultureResolver`
- New seeding infrastructure:
  - `CultureSeeder`
  - `KeySeeder`
  - `ModelAttributeSeeder`
- Startup & repair hosted services:
  - `LocalizationStartupHostedService`
  - `LocalizationRepairHostedService`
- `LocalizationKeyScanner` for model-based key discovery.
- Improved documentation:
  - Updated `README.md`
  - `BREAKING_CHANGES.md`
  - NuGet- and GitHub-ready structure.

### Changed

- Core library is now **strictly UI-agnostic**:
  - No controllers, Razor components, or JS in `Geaux.Localization`.
- Export/import logic consolidated into:
  - `LocalizationExportService`
  - `LocalizationImportService`
- EF Core configuration reorganized under `EFCore/`:
  - `EntityConfiguration`
  - `Interceptors`
  - `Seeding`
  - `Startup`.

### Fixed

- Consistent naming and placement for:
  - DbContext
  - Entity configurations
  - Seeding and hosted services.
- Improved discoverability and contributor experience via folder structure.

---

## [1.3.0] – 2025-01-22

### Added

- Tenant-aware localization via `TenantId`.
- Unique index on `(TenantId, Culture, Key)`.
- New `AddGeauxLocalization(Action<GeauxLocalizationOptions>)` overloads.
- Comprehensive behavior tests for:
  - Database-backed localizer.
  - Culture overrides.
  - Tenant scoping.
  - Localization seeding idempotency.
- XML documentation for all public APIs.

### Changed

- `Geaux.Localization` is now **fully self-contained**.
- Explicit NuGet dependencies (no `Microsoft.AspNetCore.App`).
- `LocalizedAttribute` properties are now writable for named arguments.
- Design-time DbContext factory now uses environment variable:
  - `GEAUX_LOCALIZATION_CONNECTION`.

### Fixed

- EF Core expression tree error caused by named arguments.
- FluentAssertions compatibility issue.
- Dependency injection overload mismatch.
- Invalid attribute named argument definitions.

### Removed

- Duplicate `GeauxLocalizationOptions` type.
- Implicit HttpContext dependency.
