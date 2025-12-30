# Geaux.Localization

Database-backed localization for .NET applications with:

- EF Core storage
- Optional tenant-aware behavior (integrates with Geaux.Tenant)
- Automatic migrations at startup (dev-friendly, production-capable)
- Automatic key seeding from `[Localized]` attributes

## Install

Add the core package:

```bash
dotnet add package Geaux.Localization
 ```

Add EF Core provider package(s) as needed (SQL Server recommended for production):

``` bash 
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

Quick start

In Program.cs:

```
builder.Services.AddGeauxLocalization(builder.Configuration, options =>
{
    options.ConnectionStringName = "LocalizationConnection";
    options.MigrationsAssembly = "Geaux.Localization";
    options.AutoMigrate = true;
    options.UseDbContextFactory = true;

    options.AutoSeedLocalizedAttributes = true;
    options.ModelTypes = new[] { typeof(Product), typeof(Order) };
    options.SupportedCultures = new[] { "en-US", "es-ES" };
});
 ```

Add a connection string:
 ```
{
  "ConnectionStrings": {
    "LocalizationConnection": "Server=(localdb)\\MSSQLLocalDB;Database=GeauxLocalization;Trusted_Connection=True;"
  }
}
 ```

Next: see Getting Started
.


---

## 5) `docs/articles/toc.yml`

```yml
- name: Getting Started
  href: getting-started.md
- name: Configuration
  href: configuration.md
- name: EF Core and Migrations
  href: efcore.md
- name: Localized Attribute Seeding
  href: seeding.md
- name: Tenancy Integration
  href: tenancy.md
- name: Admin UI Integration
  href: admin-ui.md
- name: Troubleshooting
  href: troubleshooting.md
- name: Changelog
  href: changelog.md
