using Geaux.Localization.Contexts;
using Geaux.Localization.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Reflection;
using LocalizationOptions = Geaux.Localization.Config.LocalizationOptions;

namespace Geaux.Localization.Extensions;

/// <summary>
/// Dependency injection helpers for registering Geaux.Localization.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Geaux.Localization using configuration binding.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configurationSection">Configuration section containing localization settings (e.g. <c>Localization</c>).</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddGeauxLocalization(this IServiceCollection services, IConfiguration configurationSection)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configurationSection == null) throw new ArgumentNullException(nameof(configurationSection));

        LocalizationOptions options = new LocalizationOptions();
        configurationSection.Bind(options);

        // Connection strings are typically stored on the configuration root.
        IConfiguration? root = TryGetConfigurationRoot(configurationSection);
        return services.AddGeauxLocalization(options, root ?? configurationSection);
    }

    /// <summary>
    /// Adds Geaux.Localization using an options configurator delegate and configuration for connection-string lookup.
    /// </summary>
    public static IServiceCollection AddGeauxLocalization(
        this IServiceCollection services,
        Action<LocalizationOptions> configureOptions,
        IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        LocalizationOptions options = new LocalizationOptions();
        configureOptions(options);

        return services.AddGeauxLocalization(options, configuration);
    }

    /// <summary>
    /// Adds Geaux.Localization using an options configurator delegate without configuration-based connection-string lookup.
    /// </summary>
    /// <remarks>
    /// This overload requires <see cref="LocalizationOptions.ConnectionString"/> to be set by <paramref name="configureOptions"/>.
    /// </remarks>
    public static IServiceCollection AddGeauxLocalization(
        this IServiceCollection services,
        Action<LocalizationOptions> configureOptions)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

        LocalizationOptions options = new LocalizationOptions();
        configureOptions(options);

        return services.AddGeauxLocalization(options, configuration: null);
    }

    /// <summary>
    /// Adds Geaux.Localization using explicit options.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="options">Localization options.</param>
    /// <param name="configuration">Optional configuration used for connection-string lookup.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddGeauxLocalization(this IServiceCollection services, LocalizationOptions options, IConfiguration? configuration = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (options == null) throw new ArgumentNullException(nameof(options));

        services.AddSingleton(Microsoft.Extensions.Options.Options.Create(options));

        // Resolve connection string
        string? resolvedConn = null;
        if (!string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            resolvedConn = options.ConnectionString;
        }
        else
        {
            var name = options.ConnectionStringName ?? "LocalizationDb";
            if (configuration != null)
            {
                resolvedConn = configuration.GetConnectionString(name);
            }

            if (string.IsNullOrWhiteSpace(resolvedConn))
                throw new InvalidOperationException($"Localization connection string '{name}' was not found.");
        }

        // Register DbContextFactory for the localization database
        services.AddDbContextFactory<GeauxLocalizationDbContext>(builder =>
        {
            var provider = (options.Provider ?? "SqlServer").Trim();
            var providerKey = provider.ToLowerInvariant();

            switch (providerKey)
            {
                case "sqlserver":
                    builder.UseSqlServer(resolvedConn, b =>
                        b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));
                    break;

                case "sqlite":
                    builder.UseSqlite(resolvedConn, b =>
                        b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));
                    break;

                case "postgresql":
                case "postgres":
                case "npgsql":
                    builder.UseNpgsql(resolvedConn, b =>
                        b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));
                    break;

                case "mysql":
                case "mariadb":
                    builder.UseMySql(resolvedConn, ServerVersion.AutoDetect(resolvedConn), b =>
                        b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));
                    break;

                default:
                    throw new InvalidOperationException($"Unknown localization provider '{provider}'.");
            }
        });

        // Register IStringLocalizerFactory as singleton (safe because it uses IDbContextFactory)
        services.AddSingleton<IStringLocalizerFactory, DatabaseStringLocalizerFactory>();

        // Interceptor (can be added to other DbContexts)
        services.AddScoped<LocalizationSaveChangesInterceptor>();

        // Defaults (optional)
        services.AddSingleton<DefaultCultureContext>();
        services.AddSingleton<NullTenantContext>();

        return services;
    }

    private static IConfiguration? TryGetConfigurationRoot(IConfiguration configuration)
    {
        if (configuration is IConfigurationRoot root)
            return root;

        Type type = configuration.GetType();

        // Property: Root (internal in some versions)
        PropertyInfo? prop = type.GetProperty("Root", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (prop != null && typeof(IConfigurationRoot).IsAssignableFrom(prop.PropertyType))
        {
            if (prop.GetValue(configuration) is IConfigurationRoot pr)
                return pr;
        }

        // Field: _root (private in some versions)
        FieldInfo? field = type.GetField("_root", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field != null && typeof(IConfigurationRoot).IsAssignableFrom(field.FieldType))
        {
            if (field.GetValue(configuration) is IConfigurationRoot fr)
                return fr;
        }

        return null;
    }
}
