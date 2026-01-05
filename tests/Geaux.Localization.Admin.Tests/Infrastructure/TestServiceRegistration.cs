using Geaux.Localization.Admin.Services;
using Geaux.Localization.Contexts;
using Geaux.Localization.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Geaux.Localization.Admin.Tests.Infrastructure;

public static class TestServiceRegistration
{
    public static void AddAdminTestServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<GeauxLocalizationDbContext>(options =>
        {
            options.UseInMemoryDatabase($"geaux-admin-tests-{Guid.NewGuid()}");
        });

        services.AddScoped<KeyAdminService>();
        services.AddScoped<CultureAdminService>();
        services.AddScoped<KeyAdminService>();

        services.AddScoped<ITenantProvider, TestTenantProvider>();
    }
}

public sealed class TestTenantProvider : ITenantProvider
{
    public string? GetTenantId() => null;
}
