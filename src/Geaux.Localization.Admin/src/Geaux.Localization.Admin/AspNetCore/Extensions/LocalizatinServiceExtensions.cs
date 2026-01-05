using Geaux.Localization.Admin.Services;
using Geaux.Localization.Config;
using Geaux.Localization.Contexts;
using Geaux.Localization.Services.Engine;
using Geaux.Shared.Activity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Geaux.Localization.EFCore.Seeding.KeySeeder;

namespace Geaux.Localization.Admin.AspNetCore.Extensions;

public static class LocalizationServiceExtensions
{
    public static IServiceCollection AddGeauxLocalizationServices(
        this IServiceCollection services,
        IConfiguration config)
    {
        // 1. Load options
        GeauxLocalizationOptions options = new GeauxLocalizationOptions();
        config.GetSection("Localization").Bind(options);

        // 2. Register DbContext
        services.AddDbContext<GeauxLocalizationDbContext>(db =>
        {
            db.UseSqlServer(options.ConnectionString, sql =>
            {
                sql.MigrationsAssembly(options.MigrationsAssembly);
            });
        });

        // 3. Register repositories
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        //services.AddScoped<LocalizationRepository>();

        // 4. Register admin services
        services.AddScoped<KeyAdminService>();
        services.AddScoped<CultureAdminService>();
        services.AddScoped<LanguagePackAdminService>();
        services.AddScoped<MaintenanceService>();
        services.AddScoped<ActivityLogRepository>();

        // 5. Register dashboard service
        services.AddScoped<DashboardService>();

        // 6. Register localizer + factory
        services.AddSingleton<LocalizationStringLocalizerFactory>();
        services.AddScoped<LocalizationStringLocalizer>();

        // 7. Register seeding
        services.AddScoped<LocalizationSeeder>();

        return services;
    }
}
