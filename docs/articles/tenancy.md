# Tenancy Integration

Geaux.Localization can participate in a tenant-aware architecture.

## How tenant scope works

When `TenantId` is set (or resolved via a tenant context in your app), localization keys and translations can be stored and queried in tenant scope.

This allows:
- per-tenant overrides of translation values
- shared “global” defaults with tenant-specific replacements

## Recommended approach

In a multi-tenant app:
1. Configure Geaux.Tenant to resolve the current tenant.
2. Configure Geaux.Localization to align with the same tenant identifier strategy.

If your app supports both:
- global keys
- tenant keys

Prefer:
- global seed for base content
- tenant seed for tenant overrides only