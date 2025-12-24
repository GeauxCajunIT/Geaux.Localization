using Geaux.Localization.Contexts;
using Geaux.Localization.Extensions;
using Geaux.Localization.SampleBlazor.Components;
using Geaux.Localization.SampleBlazor.Models;
using Geaux.Localization.SampleBlazor.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Globalization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

IConfigurationSection loc = builder.Configuration.GetSection("Localization");
Console.WriteLine("Provider=" + loc["Provider"]);
Console.WriteLine("Conn=" + loc["ConnectionString"]);

builder.Services.AddGeauxLocalization(builder.Configuration.GetSection("Localization"));

builder.Services.AddScoped<TranslationAdminService>();

// IMPORTANT: Do NOT hard-set thread culture globally here if you’re relying on RequestLocalization.
// Remove these if you have them:
// CultureInfo.DefaultThreadCurrentCulture = ...
// CultureInfo.DefaultThreadCurrentUICulture = ...

WebApplication app = builder.Build();

app.UseStaticFiles();

app.UseAntiforgery();

// Configure supported cultures
CultureInfo[] supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("fr-FR")
};

RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures,

    // Cookie FIRST so it persists and won’t revert.
    RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new QueryStringRequestCultureProvider()
    }
};

app.UseRequestLocalization(localizationOptions);

// Endpoint to set culture cookie and redirect back
app.MapGet("/culture/set", (HttpContext http, string culture, string? returnUrl) =>
{
    if (string.IsNullOrWhiteSpace(culture))
        culture = "en-US";

    RequestCulture requestCulture = new RequestCulture(culture);

    http.Response.Cookies.Append(
        CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(requestCulture),
        new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(1),
            IsEssential = true,
            Path = "/"
        });

    // Return to same page by default
    if (string.IsNullOrWhiteSpace(returnUrl) || !Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        returnUrl = "/";

    return Results.LocalRedirect(returnUrl);
});

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await InitializeLocalizationDbAsync(app);

using (IServiceScope scope = app.Services.CreateScope())
{
    IDbContextFactory<GeauxLocalizationDbContext> factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<GeauxLocalizationDbContext>>();

    await LocalizedAttributeSeeder.SeedAsync(
        factory,
        modelTypes: new[] { typeof(Product), typeof(Order) },
        supportedCultures: new[] { "en-US", "fr-FR" },
        tenantId: null);
}

app.Run();


static async Task InitializeLocalizationDbAsync(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();

    IDbContextFactory<GeauxLocalizationDbContext> factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<GeauxLocalizationDbContext>>();
    await using GeauxLocalizationDbContext db = await factory.CreateDbContextAsync();

    var provider = db.Database.ProviderName ?? "";
    var isSqlite = provider.Contains("Sqlite", StringComparison.OrdinalIgnoreCase);
    var isSqlServer = provider.Contains("SqlServer", StringComparison.OrdinalIgnoreCase);

    if (isSqlite)
    {
        await db.Database.EnsureCreatedAsync();
        return;
    }

    // SQL Server (and others): migrate if migrations exist; otherwise ensure-created
    IEnumerable<string> migrations = db.Database.GetMigrations();
    if (migrations.Any())
        await db.Database.MigrateAsync();
    else
        await db.Database.EnsureCreatedAsync();
}

