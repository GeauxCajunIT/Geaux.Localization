# Changelog
All notable changes to this project will be documented in this file.

The format is based on Keep a Changelog
and this project adheres to Semantic Versioning.

## [1.3.0] – 2025-01-22

### Added
- Tenant-aware localization via `TenantId`
- Unique index on `(TenantId, Culture, Key)`
- New `AddGeauxLocalization(Action<LocalizationOptions>)` overloads
- Comprehensive behavior tests for:
  - Database-backed localizer
  - Culture overrides
  - Tenant scoping
  - Localization seeding idempotency
- XML documentation for all public APIs

### Changed
- `Geaux.Localization` is now **fully self-contained**
- Explicit NuGet dependencies (no `Microsoft.AspNetCore.App`)
- `LocalizedAttribute` properties are now writable for named arguments
- Design-time DbContext factory now uses environment variable:
  - `GEAUX_LOCALIZATION_CONNECTION`

### Fixed
- EF Core expression tree error caused by named arguments
- FluentAssertions compatibility issue
- Dependency injection overload mismatch
- Invalid attribute named argument definitions

### Removed
- Duplicate `LocalizationOptions` type
- Implicit HttpContext dependency
