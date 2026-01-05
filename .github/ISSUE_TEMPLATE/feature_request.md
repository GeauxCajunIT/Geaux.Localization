---
name: "✨ Feature Request"
about: Suggest a new idea or enhancement for Geaux.Localization
title: "[Feature]: "
labels: enhancement
assignees: ""
---

# ✨ Feature Request

Thank you for taking the time to suggest a feature!  
Your ideas help shape the future of **Geaux.Localization**.

---

## 📌 Summary

Provide a clear and concise description of the feature you’d like to see.

> _Example: Add support for exporting language packs directly to Azure Blob Storage._

---

## 🎯 Problem or Use Case

What problem does this feature solve?  
Why is it valuable?

> _Example: In multi-tenant environments, we need a way to sync translations across distributed services._

---

## 🧠 Proposed Solution

Describe your idea for how the feature should work.

> _Example: Add an `ILanguagePackStorageProvider` interface with built-in Azure and AWS implementations._

If you have API suggestions, include them:

```csharp
public interface ILanguagePackStorageProvider
{
    Task UploadAsync(LanguagePack pack);
    Task<LanguagePack> DownloadAsync(string culture);
}
```
🔄 Alternatives Considered
Have you thought of other approaches?

Example: We could manually export/import using the admin UI, but automation would be better.

📚 Additional Context
Add any extra details, screenshots, diagrams, or references that help explain the request.

Example: This would integrate well with the existing export/import services.

✔ Checklist
[ ] I’ve checked existing issues to avoid duplicates

[ ] I’ve described the problem clearly

[ ] I’ve included a proposed solution or direction

[ ] I’ve added context or examples where helpful

Thank you for helping improve Geaux.Localization!
