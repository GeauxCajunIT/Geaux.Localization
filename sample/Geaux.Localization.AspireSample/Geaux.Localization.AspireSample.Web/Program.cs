using Geaux.Localization.Admin.Extensions;
using Geaux.Localization.AspireSample.Web.Components;
using Geaux.Localization.Extensions;
using Geaux.Shared.UI;
using MudBlazor.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ------ Geaux.Localization services --------------
builder.Services.AddGeauxLocalizationCore(builder.Configuration);
builder.Services.AddGeauxLocalizationAdmin();
//// -------------------------------------------------

// Blazor + MudBlazor
builder.Services.AddMudServices();
builder.Services.AddScoped<ThemeService>();

WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapStaticAssets();
app.UseStaticFiles();

//// ----- Geaux.Localization --------------------------------
app.MapGeauxLocalizationAdminEndpoints();
// ---------------------------------------------------------

app.MapRazorPages();

//app.MapFallbackToPage("/_Host");

// Razor Components root + Admin RCL integration
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
