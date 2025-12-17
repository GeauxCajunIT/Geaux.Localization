using Geaux.Localization.Config;
using Geaux.Localization.Data;
using Geaux.Localization.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using LocalizationOptions = Geaux.Localization.Config.LocalizationOptions;

namespace Geaux.Localization.Extensions;

/// <summary>
/// Extension methods for registering Geaux localization services with a dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Geaux localization DbContext, options, and string localizer using settings from configuration,
    /// including database provider and migrations assembly.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the localization database connection string cannot be resolved.</exception>
    public static IServiceCollection AddGeauxLocalization(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind options once so we can read Provider & MigrationsAssembly
        var options = new LocalizationOptions();
        configuration.GetSection("Localization").Bind(options);

        services.Configure<LocalizationOptions>(
            configuration.GetSection("Localization"));

        // Resolve connection string name from options, fallback to "LocalizationDb"
        var connectionStringName =
            !string.IsNullOrWhiteSpace(options.ConnectionStringName)
                ? options.ConnectionStringName
                : "LocalizationDb";

        var connString = configuration.GetConnectionString(connectionStringName);
        if (string.IsNullOrWhiteSpace(connString))
        {
            throw new InvalidOperationException(
                $"Localization connection string '{connectionStringName}' was not found.");
        }

        // Optional migrations assembly (typically Geaux.Migrations)
        var migrationsAssembly = string.IsNullOrWhiteSpace(options.MigrationsAssembly)
            ? null
            : options.MigrationsAssembly;

        // Configure DbContext for the selected provider
        services.AddDbContext<GeauxLocalizationContext>(dbOptions =>
        {
            switch (options.Provider?.Trim().ToLowerInvariant())
            {
                case "sqlserver":
                case "localdb":
                    dbOptions.UseSqlServer(
                        connString,
                        sql =>
                        {
                            if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                            }
                        });
                    break;

                case "postgresql":
                case "npgsql":
                    dbOptions.UseNpgsql(
                        connString,
                        npgsql =>
                        {
                            if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                            {
                                npgsql.MigrationsAssembly(migrationsAssembly);
                            }
                        });
                    break;

                case "mysql":
                case "mariadb":
                    dbOptions.UseMySql(
                        connString,
                        ServerVersion.AutoDetect(connString),
                        my =>
                        {
                            if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                            {
                                my.MigrationsAssembly(migrationsAssembly);
                            }
                        });
                    break;

                case "sqlite":
                    dbOptions.UseSqlite(
                        connString,
                        sqlite =>
                        {
                            if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                            {
                                sqlite.MigrationsAssembly(migrationsAssembly);
                            }
                        });
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported localization database provider '{options.Provider}'. " +
                        "Supported values: SqlServer, PostgreSql, MySql, Sqlite, LocalDb.");
            }
        });

        // Database-backed string localizer factory
        services.AddSingleton<IStringLocalizerFactory, DatabaseStringLocalizerFactory>();

        // EF Core SaveChanges interceptor for syncing translations from [Localized] properties
        services.AddScoped<LocalizationSaveChangesInterceptor>();

        return services;
    }
}
