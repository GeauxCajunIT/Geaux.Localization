namespace Geaux.Localization.Services.Engine;

using GeauxLocalizationOptions = GeauxLocalizationOptions;

/// <summary>
/// Factory that creates <see cref="LocalizationStringLocalizer"/> instances.
/// </summary>
/// <remarks>
/// The factory is registered as a singleton and uses an <see cref="IDbContextFactory{TContext}"/> to safely create
/// DbContext instances for each lookup. Tenant scoping is configured via <see cref="GeauxLocalizationOptions.TenantId"/>.
/// </remarks>
public sealed class LocalizationStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _dbFactory;
    private readonly IOptions<GeauxLocalizationOptions> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationStringLocalizerFactory"/> class.
    /// </summary>
    /// <param name="dbFactory">DbContext factory used by created localizers.</param>
    /// <param name="options">Localization options used for tenant scoping.</param>
    public LocalizationStringLocalizerFactory(
        IDbContextFactory<GeauxLocalizationDbContext> dbFactory,
        IOptions<GeauxLocalizationOptions> options)
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

        return new LocalizationStringLocalizer(_dbFactory, _options, baseName, _options.Value.TenantId);
    }

    /// <inheritdoc />
    public IStringLocalizer Create(string baseName, string location)
    {
        string? tenantId = string.IsNullOrWhiteSpace(_options.Value.TenantId)
            ? null
            : _options.Value.TenantId;

        return new LocalizationStringLocalizer(_dbFactory, _options, baseName, _options.Value.TenantId);
    }
}
