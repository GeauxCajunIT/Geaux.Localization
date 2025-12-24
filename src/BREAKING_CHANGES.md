# Breaking Changes – v1.3.0

This release contains intentional breaking changes to improve correctness,
self-containment, and multi-tenant safety.

## ❗ Removed
- `Microsoft.AspNetCore.App` framework reference
- Duplicate `Options/LocalizationOptions.cs`

## ❗ Database Schema Change
- `Translation` now includes:
  - `TenantId` (nullable)
- Unique index changed from:
  - `(Culture, Key)`
  - ➜ `(TenantId, Culture, Key)`

**Migration required.**

## ❗ Attribute Contract Change
`LocalizedAttribute` properties are now writable:
- `DisplayNameKey`
- `ErrorMessageKey`
- `DisplayMessageKey`

This enables proper named-argument usage in attributes.

## ❗ Dependency Injection
If you were previously using:
```csharp
services.AddGeauxLocalization(options => { ... });
