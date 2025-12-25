Geaux.Localization – Sample Blazor Application

This project is a reference Blazor Server application demonstrating how to use Geaux.Localization with:

Database-backed localization (IStringLocalizer)

Normalized schema (LocalizationKeys + LocalizationValues)

Culture switching with persistence

MudBlazor UI

Admin CRUD for translations

Bulk import/export (CSV / JSON)

Model-based localization via [Localized] attributes

It is intended to be both:

A working demo

A copy/paste starting point for real applications

Features
🌍 Localization

Database-backed IStringLocalizer

Normalized schema (no duplicated keys)

Culture fallback (fr-FR → fr → default)

Optional tenant overrides

Cookie-based culture persistence

🧩 Admin UI

MudBlazor-based dashboard

List/search localization keys

Inline editing per culture

Global vs tenant-aware values

Create keys without importing

Bulk import/export

📦 Import / Export

CSV and JSON supported

Preview before import

Safe upsert behavior

🧠 Model Attribute Seeding

[Localized] attribute on model properties

Automatic seeding at startup

Safe overwrite control

Prerequisites

.NET 9.0

SQL Server or SQLite

Visual Studio 2022 / Rider / VS Code

Running the Sample

From the solution root:

dotnet restore
dotnet build
dotnet run --project sample/Geaux.Localization.SampleBlazor


The app will:

Create or migrate the localization database

Seed localization keys from models

Start the Blazor Server app

Configuration
appsettings.json
{
  "Localization": {
    "Provider": "SqlServer",
    "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=LocalizationDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "DefaultCulture": "en-US",
    "TenantId": null
  }
}


Supported providers:

SqlServer

Sqlite

PostgreSql

MySql

SQLite will auto-create the database.
SQL Server will apply migrations automatically at startup.

Culture Switching

The app uses ASP.NET Core RequestLocalization.

Culture is selected from the AppBar

Selection is stored in a cookie

Culture persists across reloads and sessions

Endpoint used internally:

/culture/set?culture=fr-FR&returnUrl=/

Admin Translations UI

Navigate to:

/admin/translations

What you can do

Search localization keys

Edit values per culture

Switch between global and tenant scope

Create new keys

Import / export translations

Import Formats
CSV
Key,Culture,TenantId,Value
Home.Title,en-US,,Home
Home.Title,fr-FR,,Accueil
Product.Name,en-US,,Product

JSON
[
  {
    "key": "Home.Title",
    "culture": "en-US",
    "tenantId": null,
    "value": "Home"
  },
  {
    "key": "Home.Title",
    "culture": "fr-FR",
    "tenantId": null,
    "value": "Accueil"
  }
]


TenantId may be null or omitted

Existing values are upserted

Model-Based Localization

This sample includes two models:

Product

Order

Example:

public class Product
{
    [Localized(
        key: "Product.Name",
        displayNameKey: "Product.Name.Display",
        displayMessageKey: "Product.Name.Description")]
    public string Name { get; set; } = string.Empty;
}


At startup, these attributes are scanned and seeded into the database.

Seeder location:

Services/LocalizedAttributeSeeder.cs


Seeder is invoked in Program.cs:

await LocalizedAttributeSeeder.SeedAsync(
    factory,
    modelTypes: new[] { typeof(Product), typeof(Order) },
    supportedCultures: new[] { "en-US", "fr-FR" },
    tenantId: null);

Database Schema (Normalized)
LocalizationKeys
Column	Description
Id	Primary key
Key	Unique resource key
IsSystem	System-managed flag
LocalizationValues
Column	Description
Id	Primary key
LocalizationKeyId	FK to LocalizationKeys
Culture	Culture code (en-US)
TenantId	Optional tenant
Value	Localized text
Why Normalized?

No duplicated keys per culture

Clean tenant overrides

Faster lookups

Safer imports

Easier admin UI

Project Structure
Components/
  Pages/
    Home.razor
    Admin/Translations.razor
  Dialogs/
    ImportTranslationsDialog.razor
Services/
  TranslationAdminService.cs
  DownloadService.cs
Models/
  Product.cs
  Order.cs

Notes

This is a reference implementation

You are encouraged to copy and adapt patterns

The admin UI is intentionally simple and extensible

Related Projects

Geaux.Localization (NuGet package)

MudBlazor

License

MIT
© Geaux Cajun IT