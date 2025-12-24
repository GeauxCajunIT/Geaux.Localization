using Geaux.Localization.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using LocalizationOptions = Geaux.Localization.Config.LocalizationOptions;

namespace Geaux.Localization.Services;

/// <summary>
/// Factory that creates <see cref="DatabaseStringLocalizer"/> instances.
/// </summary>
/// <remarks>
/// The factory is registered as a singleton and uses an <see cref="IDbContextFactory{TContext}"/> to safely create
/// DbContext instances for each lookup. Tenant scoping is configured via <see cref="LocalizationOptions.TenantId"/>.
/// </remarks>
public sealed class DatabaseStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _dbFactory;
    private readonly IOptions<LocalizationOptions> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseStringLocalizerFactory"/> class.
    /// </summary>
    /// <param name="dbFactory">DbContext factory used by created localizers.</param>
    /// <param name="options">Localization options used for tenant scoping.</param>
    public DatabaseStringLocalizerFactory(
        IDbContextFactory<GeauxLocalizationDbContext> dbFactory,
        IOptions<LocalizationOptions> options)
    {
        _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public IStringLocalizer Create(Type resourceSource)
    {
        string baseName = resourceSource.FullName ?? resourceSource.Name;
        string? tenantId = string.IsNullOrWhiteSpace(_options.Value.TenantId)
            ? null
            : _options.Value.TenantId;

        return new DatabaseStringLocalizer(_dbFactory, _options, baseName, _options.Value.TenantId);
    }

    /// <inheritdoc />
    public IStringLocalizer Create(string baseName, string location)
    {
        string? tenantId = string.IsNullOrWhiteSpace(_options.Value.TenantId)
            ? null
            : _options.Value.TenantId;

        return new DatabaseStringLocalizer(_dbFactory, _options, baseName, _options.Value.TenantId);
    }
}
