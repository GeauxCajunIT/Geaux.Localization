using Geaux.Localization.Admin.Services;
using Geaux.Shared.Activity;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace Geaux.Localization.Admin.Extensions
{
    /// <summary>
    /// Provides extension methods for registering GeauxLocalization admin services with an <see
    /// cref="IServiceCollection"/>.
    /// </summary>
    public static class GeauxLocalizationAdminExtensions
    {
        /// <summary>
        /// Adds GeauxLocalization admin services to the specified service collection.
        /// </summary>
        /// <remarks>This method registers the required services for GeauxLocalization administration with
        /// scoped lifetimes. Call this method during application startup to enable localization administration
        /// features.</remarks>
        /// <param name="services">The service collection to which the admin services will be added. Cannot be null.</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/> that was provided, to support method chaining.</returns>
        public static IServiceCollection AddGeauxLocalizationAdmin(this IServiceCollection services)
        {
            // MudBlazo Services
            services.AddMudServices();

            // Geaux Localization Admin Services
            services.AddScoped<CultureAdminService>();
            services.AddScoped<KeyAdminService>();
            services.AddScoped<LanguagePackAdminService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();

            return services;
        }

    }
}
