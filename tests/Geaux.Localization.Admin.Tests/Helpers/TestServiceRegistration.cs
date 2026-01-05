using Geaux.Localization.Admin.Services;
using Geaux.Localization.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class TestServiceRegistration
{
    public static void AddAdminServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<GeauxLocalizationDbContext>(o =>
            o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddScoped<KeyAdminService>();
        services.AddScoped<CultureAdminService>();
        services.AddScoped<KeyAdminService>();
    }
}

