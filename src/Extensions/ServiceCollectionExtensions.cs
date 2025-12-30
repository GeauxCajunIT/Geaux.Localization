using Geaux.Localization.Config;
using Geaux.Localization.Contexts;
using Geaux.Localization.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Geaux.Localization.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGeauxLocalization(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<GeauxLocalizationOptions>? configure = null)
    {
        GeauxLocalizationOptions opts = new GeauxLocalizationOptions();
        configure?.Invoke(opts);

        services.AddSingleton<IOptions<GeauxLocalizationOptions>>(_ => Options.Create(opts));

        // Resolve connection string once
        string conn = ResolveConnectionString(configuration, opts);

        // Register DbContext (factory or scoped)
        if (opts.UseDbContextFactory)
        {
            services.AddDbContextFactory<GeauxLocalizationDbContext>(db =>
                ConfigureProvider(db, opts, conn));
        }
        else
        {
            services.AddDbContext<GeauxLocalizationDbContext>(db =>
                ConfigureProvider(db, opts, conn));
        }

        // Localization factory
        services.AddSingleton<IStringLocalizerFactory, DatabaseStringLocalizerFactory>();

        // Optional typed localizer adapter
        services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

        // Interceptor is optional for consumers to add to THEIR DbContexts
        services.AddSingleton<LocalizationSaveChangesInterceptor>();

        // One hosted service to rule them all
        services.AddHostedService<LocalizationStartupHostedService>();

        return services;
    }

    private static void ConfigureProvider(DbContextOptionsBuilder db, GeauxLocalizationOptions opts, string connectionString)
    {
        string? migrationsAssembly =
            string.IsNullOrWhiteSpace(opts.MigrationsAssembly)
                ? typeof(GeauxLocalizationDbContext).Assembly.GetName().Name
                : opts.MigrationsAssembly;

        string provider = (opts.Provider ?? "SqlServer").Trim().ToLowerInvariant();

        switch (provider)
        {
            case "sqlite":
                db.UseSqlite(connectionString, b =>
                {
                    if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                        b.MigrationsAssembly(migrationsAssembly);
                });
                break;

            case "postgres":
            case "postgresql":
            case "npgsql":
                db.UseNpgsql(connectionString, b =>
                {
                    if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                        b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.FullName);
                });
                break;

            case "mysql":
            case "mariadb":
                db.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b =>
                {
                    if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                        b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.FullName);
                });
                break;

            case "sqlserver":
            default:
                db.UseSqlServer(connectionString, b =>
                {
                    if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                        b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.FullName);

                    if (opts.EnableRetryOnFailure)
                        b.EnableRetryOnFailure();
                });
                break;
        }

        if (opts.ThrowOnPendingModelChanges)
        {
            db.ConfigureWarnings(w => w.Throw(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
    }

    private static string ResolveConnectionString(IConfiguration configuration, GeauxLocalizationOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
            return options.ConnectionString;

        string name = string.IsNullOrWhiteSpace(options.ConnectionStringName)
            ? "LocalizationConnection"
            : options.ConnectionStringName;

        string? fromConfig = configuration.GetConnectionString(name);
        if (!string.IsNullOrWhiteSpace(fromConfig))
            return fromConfig;

        throw new InvalidOperationException(
            $"Geaux.Localization: connection string '{name}' was not found. " +
            $"Set ConnectionStrings:{name} or set options.ConnectionString.");
    }
}
