using Geaux.Localization.Attributes;
using Geaux.Localization.Config;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Geaux.Localization.Services;

/// <summary>
/// EF Core interceptor that upserts localization keys/values for entities with properties marked by <see cref="LocalizedAttribute"/>.
/// </summary>
/// <remarks>
/// This interceptor is intended to be added to application DbContexts so that whenever entities are saved,
/// localization keys and default values are kept in sync with the localization database.
/// </remarks>
public sealed class LocalizationSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly GeauxLocalizationDbContext _localizationDb;
    private readonly LocalizationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationSaveChangesInterceptor"/> class.
    /// </summary>
    /// <param name="options">Localization options.</param>
    /// <param name="configuration">Application configuration (used for connection string resolution).</param>
    public LocalizationSaveChangesInterceptor(
        IOptions<LocalizationOptions> options,
        IConfiguration? configuration = null)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        _options = options.Value;

        DbContextOptionsBuilder<GeauxLocalizationDbContext> builder = new DbContextOptionsBuilder<GeauxLocalizationDbContext>();

        string provider = (_options.Provider ?? "SqlServer").Trim();
        string providerKey = provider.ToLowerInvariant();
        string connectionString = ResolveConnectionString(_options, configuration);

        switch (providerKey)
        {
            case "sqlserver":
                builder.UseSqlServer(connectionString, b =>
                    b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));
                break;
            case "sqlite":
                builder.UseSqlite(connectionString, b =>
                    b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));
                break;
            case "postgresql":
            case "postgres":
            case "npgsql":
                builder.UseNpgsql(connectionString, b =>
                    b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));
                break;
            case "mysql":
            case "mariadb":
                builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b =>
                    b.MigrationsAssembly(typeof(GeauxLocalizationDbContext).Assembly.GetName().Name));
                break;
            default:
                throw new InvalidOperationException($"Unknown localization provider '{provider}'.");
        }

        _localizationDb = new GeauxLocalizationDbContext(builder.Options);
    }

    private static string ResolveConnectionString(LocalizationOptions opts, IConfiguration? configuration)
    {
        if (!string.IsNullOrWhiteSpace(opts.ConnectionString))
            return opts.ConnectionString;

        string name = opts.ConnectionStringName ?? "LocalizationDb";
        if (configuration != null)
        {
            return configuration.GetConnectionString(name)
                ?? throw new InvalidOperationException($"Localization connection string '{name}' was not found.");
        }

        throw new InvalidOperationException("Localization connection string is not configured. Provide LocalizationOptions.ConnectionString or a configured connection string named in LocalizationOptions.ConnectionStringName.");
    }

    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        DbContext? context = eventData.Context;
        if (context == null) return result;

        string? tenantId = string.IsNullOrWhiteSpace(_options.TenantId) ? null : _options.TenantId;

        IEnumerable<EntityEntry> entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (EntityEntry entry in entries)
        {
            var props = entry.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                if (!Attribute.IsDefined(prop, typeof(LocalizedAttribute))) continue;

                LocalizedAttribute attr = (LocalizedAttribute)Attribute.GetCustomAttribute(prop, typeof(LocalizedAttribute))!;
                string culture = attr.Culture ?? Thread.CurrentThread.CurrentUICulture.Name;

                string value = prop.GetValue(entry.Entity)?.ToString() ?? string.Empty;
                await UpsertLocalizationAsync(tenantId, culture, attr.Key, value, cancellationToken).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(attr.ErrorMessageKey))
                {
                    await UpsertLocalizationAsync(tenantId, culture, attr.ErrorMessageKey, $"{prop.Name} is required", cancellationToken).ConfigureAwait(false);
                }

                if (!string.IsNullOrEmpty(attr.DisplayNameKey))
                {
                    await UpsertLocalizationAsync(tenantId, culture, attr.DisplayNameKey, prop.Name, cancellationToken).ConfigureAwait(false);
                }

                if (!string.IsNullOrEmpty(attr.DisplayMessageKey))
                {
                    await UpsertLocalizationAsync(tenantId, culture, attr.DisplayMessageKey, $"Info about {prop.Name}", cancellationToken).ConfigureAwait(false);
                }
            }
        }

        await _localizationDb.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    private async Task UpsertLocalizationAsync(string? tenantId, string culture, string key, string value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        // Ensure key exists
        var keyEntity = await _localizationDb.LocalizationKeys
            .FirstOrDefaultAsync(k => k.Key == key, cancellationToken)
            .ConfigureAwait(false);

        if (keyEntity == null)
        {
            keyEntity = new LocalizationKey { Key = key };
            _localizationDb.LocalizationKeys.Add(keyEntity);
            await _localizationDb.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        var existing = await _localizationDb.LocalizationValues
            .FirstOrDefaultAsync(v =>
                v.LocalizationKeyId == keyEntity.Id &&
                v.TenantId == tenantId &&
                v.Culture == culture, cancellationToken)
            .ConfigureAwait(false);

        if (existing == null)
        {
            _localizationDb.LocalizationValues.Add(new LocalizationValue
            {
                LocalizationKeyId = keyEntity.Id,
                TenantId = tenantId,
                Culture = culture,
                Value = value
            });
        }
        else if (!string.Equals(existing.Value, value, StringComparison.Ordinal))
        {
            // Interceptor keeps defaults in sync; only overwrite system values.

        }
    }
}
