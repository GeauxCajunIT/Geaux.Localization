# Admin UI Integration

Your sample provides an admin page to manage localization keys and translations.

## Common admin patterns

- Search and filter keys by:
  - prefix / resource
  - culture
  - tenant scope
- Inline edit translation values
- Export/import translations (CSV/JSON)

## Recommended services

Typical supporting services used by admin pages:
- `TranslationAdminService` (query keys, update translations)
- `DownloadService` (export)

Ensure these are registered in the sample app's DI container.