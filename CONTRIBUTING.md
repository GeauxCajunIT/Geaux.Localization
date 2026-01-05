# Contributing to Geaux.Localization

Thank you for your interest in contributing!  
Geaux.Localization is an open‑source project designed to bring high‑quality, database‑backed localization to .NET applications — and contributions from the community help it grow stronger.

Whether you're fixing a bug, improving documentation, or adding new features, this guide will help you get started.

---

## 🧭 Ways to Contribute

You can contribute in many ways:

- Reporting bugs  
- Requesting features  
- Improving documentation or examples  
- Submitting pull requests  
- Reviewing existing issues or PRs  
- Helping shape the roadmap  

All contributions are welcome — big or small.

---

## 📝 Code of Conduct

By participating in this project, you agree to uphold the standards of respectful, constructive collaboration.

Please be kind, patient, and supportive of others.

---

## Before you Start

> Make sure you have .NET 9 installed and the repo builds before making changes.

## 🛠️ Development Setup

### 1. Fork the repository

Click **Fork** on GitHub to create your own copy.

### 2. Clone your fork

```bash
git clone https://github.com/<your-username>/Geaux.Localization.git
cd Geaux.Localization
```

### 3. Create a feature branch
```bash
git checkout -b feature/my-new-feature
```

Use a descriptive branch name:

- feature/...

- fix/...

- docs/...

- refactor/...

### 4. Build the solution
```bash
dotnet build
```

### 5. Run tests
```bash
dotnet test
```
All tests should pass before submitting a PR.

## 📁 Project Structure Overview
```text
src/Geaux.Localization/        → Core library
src/Geaux.Localization.Admin/  → Admin UI (RCL)
samples/                       → Sample apps
tests/                         → Unit tests
```

The core library is UI‑agnostic and contains:

- EF Core models & DbContext

- Localization engine

- Export/import services

- Seeding & maintenance

- Attribute scanning

- Hosted services

The admin RCL contains:

- Razor components

- Admin APIs

- File upload/download

- Language pack UI

## 🧪 Testing Guidelines
- Add tests for new features

- Update tests when modifying behavior

- Keep tests small, focused, and deterministic

- Prefer unit tests over integration tests unless necessary

Test project:

```text
tests/Geaux.Localization.Tests/
```

## 📦 Pull Request Guidelines
Before opening a PR:

1. Ensure your branch is up to date with main

2. Run dotnet build and dotnet test

3. Follow existing coding style and folder structure

4. Include documentation updates if needed

5. Reference related issues in your PR description

PR Title Format
Use clear, descriptive titles:

- Add culture repair service

- Fix missing value seeding

- Improve admin UI file upload

- Update README badges

PR Review Expectations
- Reviews are friendly and constructive

- Requested changes are normal — not criticism

- Discussions are welcome

## 🐛 Reporting Bugs
If you find a bug, please open an issue with:

- A clear title

- Steps to reproduce

- Expected behavior

- Actual behavior

- Environment details (OS, .NET version, provider, etc.)

- Optional: screenshots or logs

## 💡 Requesting Features
Feature requests are welcome!

Please include:

- What problem the feature solves

- Why it’s valuable

- Any examples or scenarios

- Optional: proposed API or UI

## 🧩 Coding Style
- Use C# 12 features where appropriate

- Follow .NET naming conventions

- Keep methods small and focused

- Prefer dependency injection

- Avoid breaking changes unless intentional

- Keep the core library UI‑agnostic

## 🧰 Commit Message Style
Use conventional commits when possible:

- feat: new feature

- fix: bug fix

- docs: documentation

- refactor: code cleanup

- test: test updates

- chore: build or tooling

Examples:

```text
feat: add tenant-aware culture resolver
fix: correct CSV export ordering
docs: update README with admin UI instructions
```

## 🚀 Release Process
Releases follow semantic versioning:

- **MAJOR** — breaking changes

- **MINOR** — new features

- **PATCH** — bug fixes

Release notes are maintained in:

- CHANGELOG.md

- BREAKING_CHANGES.md

NuGet packages are published via GitHub Actions.

# ❤️ Thank You
Your contributions help make Geaux.Localization better for everyone.
Whether you're fixing a typo or building a major feature — you’re appreciated.

If you have questions, ideas, or need help getting started, feel free to open an issue or discussion.

**Happy coding!**