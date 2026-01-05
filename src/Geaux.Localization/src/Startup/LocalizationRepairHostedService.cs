using Geaux.Localization.Services.Maintenance;

namespace Geaux.Localization.Startup
{

    public sealed class LocalizationRepairHostedService : IHostedService
    {
        private readonly IServiceProvider _services;

        public LocalizationRepairHostedService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _services.CreateScope();
            LocalizationMaintenanceService maintenance = scope.ServiceProvider.GetRequiredService<LocalizationMaintenanceService>();

            // Run once at startup to ensure consistency
            await maintenance.RepairMissingValuesAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
