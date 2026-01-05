using Geaux.Localization.Admin.Services;
using Geaux.Localization.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Geaux.Localization.Admin.Tests.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAdminTestServices(this IServiceCollection services)
    {
        // In‑memory DB for admin services
        services.AddDbContext<GeauxLocalizationDbContext>(options =>
            options.UseInMemoryDatabase("GeauxLocalization_Admin_Tests"));

        services.AddScoped<CultureAdminService>();
        services.AddScoped<KeyAdminService>();

        return services;
    }
}
