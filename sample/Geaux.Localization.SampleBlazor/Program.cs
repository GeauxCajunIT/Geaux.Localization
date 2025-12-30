using Geaux.Localization.Extensions;
using Geaux.Localization.SampleBlazor.Components;
using Geaux.Localization.SampleBlazor.Models;
using Geaux.Localization.SampleBlazor.Services;
using Microsoft.AspNetCore.Localization;
using MudBlazor.Services;
using System.Globalization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Geaux.Localization
builder.Services.AddGeauxLocalization(builder.Configuration, options =>
{
    options.ConnectionStringName = "LocalizationConnection";
    options.MigrationsAssembly = "Geaux.Localization";
    options.AutoMigrate = true;
    options.ThrowOnPendingModelChanges = true;
    options.UseDbContextFactory = true;

    options.AutoSeedLocalizedAttributes = true;
    options.SeedOverwriteExisting = true;  // dev/sample convenience
    options.SupportedCultures = ["en-US", "es-ES"];
    options.ModelTypes =
    [
        typeof(Product),
        typeof(Order)
    ];
});

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

app.Run();
