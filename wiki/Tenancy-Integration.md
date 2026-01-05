# Tenancy Integration

Geaux.Localization supports tenant-aware translations while preserving global defaults.

## How lookups are resolved
1. Culture chain: the requested culture, then parent cultures when `EnableCultureFallback` is true, then `DefaultCulture` (if not already tried).
2. Tenant precedence: tenant-specific translations are preferred; global translations are used when a tenant value is missing.
3. Missing keys return the key name itself.

## Configuring tenant behavior
- Set `options.TenantId` when you want all localization lookups to target a specific tenant.
- Leave `TenantId` null to operate in the global scope; tenant-aware consumers can still create a localizer with a tenant override.
- When seeding manually, pass `tenantId` to `ModelAttributeSeeder.SeedAsync` to create tenant-specific values.

## Patterns
- **Global defaults + tenant overrides**: seed global keys/values first, then seed only the tenant overrides that differ.
- **Per-tenant isolation**: provide a `TenantId` when resolving the localizer so each tenant reads only its own values, with global fallback.
- **Admin UI**: the admin package respects tenant scoping for queries and writes.
