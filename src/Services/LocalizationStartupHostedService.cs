using Geaux.Localization.Config;
using Geaux.Localization.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Geaux.Localization.Services;

public sealed class LocalizationStartupHostedService : IHostedService
{
    private readonly IServiceProvider _sp;
    private readonly IOptions<GeauxLocalizationOptions> _options;
    private readonly ILogger<LocalizationStartupHostedService> _logger;

    public LocalizationStartupHostedService(
        IServiceProvider sp,
        IOptions<GeauxLocalizationOptions> options,
        ILogger<LocalizationStartupHostedService> logger)
    {
        _sp = sp;
        _options = options;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        using IServiceScope scope = _sp.CreateScope();
        GeauxLocalizationOptions opts = _options.Value;

        IDbContextFactory<GeauxLocalizationDbContext> factory = scope.ServiceProvider
            .GetRequiredService<IDbContextFactory<GeauxLocalizationDbContext>>();

        // 1️⃣ Ensure schema exists FIRST
        await using (GeauxLocalizationDbContext db = await factory.CreateDbContextAsync(ct))
        {
            if (opts.AutoMigrate)
            {
                _logger.LogInformation("Applying Geaux.Localization migrations...");
                await db.Database.MigrateAsync(ct);
            }
        }

        // 2️⃣ Seed AFTER schema exists
        if (opts.AutoSeedLocalizedAttributes &&
            opts.ModelTypes.Count > 0 &&
            opts.SupportedCultures.Count > 0)
        {
            _logger.LogInformation("Seeding localized attributes...");

            await LocalizedAttributeSeeder.SeedAsync(
                factory,
                opts.ModelTypes,
                opts.SupportedCultures,
                tenantId: null,
                false,
                ct);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
