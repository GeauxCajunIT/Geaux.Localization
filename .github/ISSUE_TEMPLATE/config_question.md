  ---
name: "⚙️ Configuration Question"
about: Ask for help configuring Geaux.Localization in your application
title: "[Config]: "
labels: question
assignees: ""
---

# ⚙️ Configuration Question

Thanks for reaching out!  
Please fill out the details below so we can help you quickly.

---

## 📌 What Are You Trying to Configure?

Describe what you're trying to set up:

> _Example: I’m trying to configure tenant-aware localization with SqlServer._

---

## 🧩 Current Configuration

Please paste your relevant configuration:

### appsettings.json (or equivalent)

```json
{
  "Localization": {
    "Provider": "",
    "ConnectionStringName": "",
    "DefaultCulture": "",
    "TenantId": ""
  }
}
 ```

 Service registration
```csharp
// Example:
builder.Services.AddGeauxLocalization(
    builder.Configuration.GetSection("Localization"));
```

❗ What’s Not Working?
Describe the issue you’re running into:

> Example: The default culture is applied, but tenant-specific values are not loading.

## 🔍 Expected Behavior
What did you expect to happen?

> Example: When TenantId = "tenant-1", I expected tenant-scoped translations to override global ones.

## 🧪 Environment Details
- **OS:**

- **.NET version:**

- **Database provider: (SqlServer, Sqlite, PostgreSQL, etc.)**

- **Package version:**

- **Admin UI used: Yes / No**

- **Multi‑tenant setup: Yes / No**

## 📄 Logs / Errors (Optional)
Paste any relevant logs, stack traces, or console output.

## 📚 Additional Context
Anything else that might help?

Example: I’m using a custom ITenantContext implementation.