# 📦 Geaux.Localization.Admin
A plug‑and‑play admin dashboard for managing database‑backed localization in .NET applications.

<p align="left">
<img src="https://img.shields.io/nuget/v/Geaux.Localization.Admin?color=4c1&label=NuGet%20Version" />
<img src="https://img.shields.io/nuget/dt/Geaux.Localization.Admin?color=blue&label=Downloads" />
<img src="https://img.shields.io/github/actions/workflow/status/GeauxCajunIT/Geaux.Localization/build.yml?label=Build" />
<img src="https://img.shields.io/github/license/GeauxCajunIT/Geaux.Localization?color=yellow" />
<img src="https://img.shields.io/badge/.NET-9.0-blueviolet" />
<img src="https://img.shields.io/badge/MudBlazor-8.x-1E88E5" />
</p>

-------

## 🚀 Overview
Geaux.Localization.Admin is a Razor Class Library (RCL) that provides a full administrative UI for managing everything related to localization:

- Cultures

- Keys

- Values

- Missing value pair

- Language packs

- Multi‑tenant overrides

- Maintenance and repair tools

- Export/import (CSV/JSON/ZIP)

It is designed to work seamlessly with the core Geaux.Localization package.

Built with MudBlazor and fully theme-aware.

-------

## ✨ Features
- Culture management  
Add, list, and manage supported cultures.

- Key management  
Create new localization keys and automatically seed missing values.

- Translation editor  
View and update translations across cultures and tenants.

- Export/Import tooling

	- JSON

	- CSV

	- Missing‑only CSV/JSON

	- ZIP bundles (all cultures)

- Multi‑tenant support  
All operations respect tenant scoping.

- Maintenance tools  
Repair missing values across all cultures and keys.

- Modern UI  
Built with MudBlazor for a clean, responsive experience.

-------

## 📥 Installation
```bash
dotnet add package Geaux.Localization.Admin
```
You must also install the core package:

```bash
dotnet add package Geaux.Localization
```

## 🛠️ Setup

**1. Register Admin Services **
```csharp
builder.Services.AddGeauxLocalizationAdmin();
```

**2. Map Admin Endpoints**
```csharp
app.MapGeauxLocalizationAdmin();
```

**3. Add the Admin UI to Your App**

In your Blazor or MVC app:

```csharp
@using Geaux.Localization.Admin
```
The dashboard becomes available at:

```Code
/admin/localization
```
(You can customize this route.)

## 🧩 API Endpoints
The Admin package exposes REST endpoints for:

- Cultures

- Keys

- Language packs

- Maintenance

Examples:

```text
GET  /admin/localization/api/cultures
POST /admin/localization/api/keys
GET  /admin/localization/api/language-pack/{culture}
POST /admin/localization/api/maintenance/repair
```

These power the UI and can be used programmatically.
 
-------

## 📚 Documentation
Full documentation, examples, and screenshots:

👉 https://github.com/GeauxCajunIT/Geaux.Localization/wiki
 
-------

## 🐛 Issues & Support
Open an issue:

👉 https://github.com/GeauxCajunIT/Geaux.Localization/issues
  
-------

## ❤️ Contributing
Contributions are welcome!

- CONTRIBUTING.md

- CODE_OF_CONDUCT.md

- SECURITY.md

- SUPPORT.md

-------

## 📄 License
MIT License — free for commercial and open‑source use.