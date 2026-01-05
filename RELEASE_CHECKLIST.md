# Geaux.Localization – Release Checklist

This checklist ensures each release of **Geaux.Localization** (and the Admin RCL) is consistent, stable, and well‑documented.

Follow these steps before publishing a new version to NuGet.

---

## 1. 🧹 Pre‑Release Preparation

### Verify repository cleanliness
- [ ] All changes committed  
- [ ] No leftover debug code  
- [ ] No unused files or folders  
- [ ] No TODO/FIXME comments in production code  

### Update dependencies
- [ ] Run `dotnet restore`  
- [ ] Update NuGet dependencies if needed  
- [ ] Ensure no breaking dependency changes  

---

## 2. 🧪 Testing & Validation

### Run full test suite
- [ ] `dotnet test` passes  
- [ ] New features have test coverage  
- [ ] Breaking changes have migration notes  

### Manual validation (recommended)
- [ ] Sample app builds and runs  
- [ ] Admin UI loads correctly  
- [ ] Export/import works  
- [ ] Seeding pipeline runs without errors  
- [ ] Multi‑tenant behavior verified (if applicable)  

---

## 3. 📝 Documentation Updates

### Update versioned docs
- [ ] `CHANGELOG.md` updated  
- [ ] `BREAKING_CHANGES.md` updated (if needed)  
- [ ] `README.md` updated (badges, examples, features)  
- [ ] Wiki pages updated (if applicable)  

### Verify metadata
- [ ] NuGet description accurate  
- [ ] Tags/keywords correct  
- [ ] Project URLs correct  
- [ ] License included  

---

## 4. 🔢 Versioning

### Choose correct semantic version
- [ ] **MAJOR** – breaking changes  
- [ ] **MINOR** – new features  
- [ ] **PATCH** – bug fixes  

### Update version numbers
- [ ] `Directory.Build.props` or `.csproj` version updated  
- [ ] Admin RCL version updated (if applicable)  
- [ ] Sample apps updated (if pinned)  

---

## 5. 🚀 Build & Package

### Build release artifacts
- [ ] Run `dotnet build -c Release`  
- [ ] Run `dotnet pack -c Release`  
- [ ] Verify `.nupkg` contents (no secrets, no unnecessary files)  

---

## 6. 🔐 Security & Compliance

- [ ] Review `SECURITY.md` for any updates  
- [ ] Ensure no sensitive data in commits  
- [ ] Ensure no internal URLs or credentials  
- [ ] Validate third‑party licenses  

---

## 7. 🏁 Publish Release

### GitHub Release
- [ ] Create a new GitHub Release  
- [ ] Tag version (e.g., `v2.0.0`)  
- [ ] Add release notes (copy from CHANGELOG)  
- [ ] Attach any relevant artifacts (optional)  

### NuGet Publish
- [ ] Ensure `NUGET_API_KEY` is valid in GitHub Secrets  
- [ ] Confirm GitHub Actions `publish.yml` workflow succeeded  
- [ ] Verify package appears on NuGet.org  

---

## 8. 📣 Post‑Release

- [ ] Announce release in Discussions (optional)  
- [ ] Update pinned issues or roadmap  
- [ ] Close issues resolved by this release  
- [ ] Merge `main` → `develop` (if using GitFlow)  

---

## 🎉 Done!

Thank you for maintaining **Geaux.Localization**!  
Each release helps the community build better multi‑tenant, localized .NET applications.
