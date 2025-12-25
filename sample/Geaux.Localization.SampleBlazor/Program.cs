using Geaux.Localization.Contexts;
using Geaux.Localization.Extensions;
using Geaux.Localization.SampleBlazor.Components;
using Geaux.Localization.SampleBlazor.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Globalization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Geaux.Localization (from appsettings.json -> "Localization")
builder.Services.AddGeauxLocalization(builder.Configuration.GetSection("Localization"));

// Admin CRUD service
builder.Services.AddScoped<TranslationAdminService>();
builder.Services.AddScoped<DownloadService>();

// Enable IStringLocalizer<T> injections using the registered factory
builder.Services.AddTransient(typeof(Microsoft.Extensions.Localization.IStringLocalizer<>), typeof(TypedStringLocalizer<>));

WebApplication app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();

// Supported cultures
CultureInfo[] supportedCultures =
{
    new("en-US"),
    new("fr-FR"),
};

RequestLocalizationOptions localizationOptions = new()
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,

    // COOKIE FIRST so it persists and doesn't revert
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    }
};

app.UseRequestLocalization(localizationOptions);

// Endpoint: set culture cookie and redirect back
app.MapGet("/culture/set", (HttpContext http, string culture, string? returnUrl) =>
{
    if (string.IsNullOrWhiteSpace(culture))
        culture = "en-US";

    RequestCulture requestCulture = new(culture);

    http.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(requestCulture),
        new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            Path = "/"
        });

    if (string.IsNullOrWhiteSpace(returnUrl) || !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        returnUrl = "/";

    return Results.LocalRedirect(returnUrl);
});

// Blazor
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// Initialize database provider behavior:
// - SQLite: auto-create (EnsureCreated)
// - SQL Server: migrate automatically (in Dev/Staging/Prod)
await InitializeLocalizationDbAsync(app);

// Optional: seed LocalizedAttribute keys (if you're using that seeder in the sample)
// await SeedLocalizedAttributesAsync(app, supportedCultures.Select(c => c.Name));

app.Run();

static async Task InitializeLocalizationDbAsync(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();

    IDbContextFactory<GeauxLocalizationDbContext> factory =
        scope.ServiceProvider.GetRequiredService<IDbContextFactory<GeauxLocalizationDbContext>>();

    await using GeauxLocalizationDbContext db = await factory.CreateDbContextAsync();

    string provider = db.Database.ProviderName ?? string.Empty;

    bool isSqlite = provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase);
    bool isSqlServer = provider.Contains("SqlServer", StringComparison.OrdinalIgnoreCase);

    if (isSqlite)
    {
        // SQLite: always auto-create for sample/dev convenience
        await db.Database.EnsureCreatedAsync();
        return;
    }

    if (isSqlServer)
    {
        // SQL Server: migrate automatically (all envs); this is the "real" behavior you want.
        // If no migrations exist, MigrateAsync won't apply anything but still validates setup.
        await db.Database.MigrateAsync();
        return;
    }

    // Fallback for other providers
    IEnumerable<string> pending = await db.Database.GetPendingMigrationsAsync();
    if (pending.Any())
        await db.Database.MigrateAsync();
    else
        await db.Database.EnsureCreatedAsync();
}

// If you're using LocalizedAttribute seeding, wire this back in
// static async Task SeedLocalizedAttributesAsync(WebApplication app, IEnumerable<string> cultures)
// {
//     using var scope = app.Services.CreateScope();
//     var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<GeauxLocalizationDbContext>>();
//     await LocalizedAttributeSeeder.SeedAsync(
//         factory,
//         modelTypes: new[] { typeof(Geaux.Localization.SampleBlazor.Models.Product), typeof(Geaux.Localization.SampleBlazor.Models.Order) },
//         supportedCultures: cultures,
//         tenantId: null,
//         ct: default);
// }
