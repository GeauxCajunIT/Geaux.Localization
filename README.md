Geaux.Localization

Tenant-aware, culture-aware, database-backed localization for .NET 8+ SaaS applications.

Geaux.Localization provides a complete end-to-end localization system for multi-tenant and multi-culture applications.
It integrates deeply with EF Core, ASP.NET Core, and the Geaux Platform's tenant and administrative systems.

✨ Features

🌍 Database-backed IStringLocalizer

🏬 Tenant-aware (isolated translations per tenant)

🗣 Culture-aware with fallback support

🧩 [Localized] attribute for annotating model properties

🔄 Automatic translation syncing (via EF Core SaveChanges interceptor)

🛠 CLI tool for scanning, seeding, and syncing translations

🧰 Multi-database support:

SQL Server

PostgreSQL

MySQL/MariaDB

Sqlite

LocalDB

📦 NuGet-ready, standalone library

⚙ Compatible with .NET 8, .NET 9, future .NET versions

📦 Installation
Library
dotnet add package Geaux.Localization

EF Core Extensions
dotnet add package Geaux.Localization.EntityFrameworkCore

CLI Tool
dotnet tool install --global Geaux.Localization.CLI

🚀 Getting Started
1. Register services

Add to your Program.cs:

builder.Services.AddGeauxLocalization(builder.Configuration);


This registers:

LocalizationOptions

GeauxLocalizationContext

DatabaseStringLocalizer

DatabaseStringLocalizerFactory

LocalizationSaveChangesInterceptor

💾 Configuration

Add to appsettings.json:

{
  "ConnectionStrings": {
    "LocalizationDb": "Server=localhost;Database=GeauxLocalization;Trusted_Connection=True;"
  },
  "Localization": {
    "DefaultCulture": "en-US",
    "SupportedCultures": [ "en-US", "fr-FR" ],
    "EnableCultureFallback": true,
    "ConnectionStringName": "LocalizationDb",
    "Provider": "SqlServer",
    "MigrationsAssembly": "Geaux.Migrations"
  }
}

🧊 Database Providers Supported

Set via "Provider" in your config:

Provider	EF Core Call	Example
SQL Server	UseSqlServer	"Provider": "SqlServer"
PostgreSQL	UseNpgsql	"Provider": "PostgreSql"
MySQL/MariaDB	UseMySql	"Provider": "MySql"
Sqlite	UseSqlite	"Provider": "Sqlite"
LocalDB	SQL Server provider	"Provider": "LocalDb"

Migrations usually live in:
Geaux.Migrations

🧩 LocalizedAttribute

Annotate a property to automatically generate translation keys.

Example
[Localized(
    key: "Role.Name",
    errorMessageKey: "Role.Name.Required",
    displayNameKey: "Role.Name.Label",
    displayMessageKey: "Role.Name.Info")]
public new string Name { get; set; } = string.Empty;


Keys generated:

Role.Name
Role.Name.Required
Role.Name.Label
Role.Name.Info


These are stored as rows in the Translation table:

TenantId | Culture | Key                      | Value

Works with:

Admin panels

Forms

DTO validation

Display labels

Tooltips

Help text

All unified under consistent translation keys.

🏗 How Localization Works
1️⃣ At runtime

DatabaseStringLocalizer resolves:

Active tenant (via ITenantProvider)

Active culture (via ASP.NET RequestLocalization)

Translation lookup → (TenantId, Culture, Key)

Optional fallback to default culture

2️⃣ At save time

LocalizationSaveChangesInterceptor:

Detects [Localized] annotations

Creates or updates translation rows

Ensures keys remain in sync with domain models

3️⃣ At build/CI time

Geaux.Localization.CLI:

Scans assemblies

Finds [Localized] properties

Seeds missing translations

Supports export/import for translators

📘 Example Entity With Localization
using Geaux.Localization.Attributes;
using Microsoft.AspNetCore.Identity;

public class GeauxIdentityRole : IdentityRole
{
    public string TenantId { get; set; } = "default";

    [Localized(
        key: "Role.Name",
        errorMessageKey: "Role.Name.Required",
        displayNameKey: "Role.Name.Label",
        displayMessageKey: "Role.Name.Info")]
    public new string Name { get; set; } = string.Empty;

    [Localized(
        key: "Role.Description",
        errorMessageKey: "Role.Description.Required",
        displayNameKey: "Role.Description.Label",
        displayMessageKey: "Role.Description.Info")]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}

🧰 Project Structure
Geaux.Localization/
│
├── Attributes/
│   └── LocalizedAttribute.cs
│
├── Config/
│   └── LocalizationOptions.cs
│
├── Data/
│   └── GeauxLocalizationContext.cs
│
├── Models/
│   └── Translation.cs
│
├── Services/
│   ├── DatabaseStringLocalizer.cs
│   ├── DatabaseStringLocalizerFactory.cs
│   ├── LocalizationResolver.cs
│   └── LocalizationSaveChangesInterceptor.cs
│
├── Extensions/
│   └── ServiceCollectionExtensions.cs
│
├── README.md
└── Geaux.Localization.csproj


CLI project:

Geaux.Localization.CLI/

📄 License

MIT License
Copyright ©
Brent Lee Rigsby / GeauxCajunIT

🌐 Repository

GitHub:
https://github.com/GeauxCajunIT/GeauxPlatform