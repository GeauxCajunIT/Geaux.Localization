# Geaux.Shared

This project is a **source-only shared library** used by applications that integrate
Geaux.Localization and Geaux.Localization.Admin.

It is intentionally **NOT** published to NuGet so that each application can:

- Customize models
- Extend interfaces
- Add additional fields
- Modify behavior
- Add tenant/user context logic

## How to use

Copy the entire `Geaux.Shared` folder into your solution:
```text
/src/YourApp/Geaux.Shared/
 ```

Then reference it:

```xml
<ProjectReference Include="..\Geaux.Shared\Geaux.Shared.csproj" />
```
You may freely modify any file.
