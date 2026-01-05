using Geaux.Localization.Services.Maintenance;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("admin/localization/api/maintenance")]
public sealed class LocalizationMaintenanceController : ControllerBase
{
    private readonly LocalizationMaintenanceService _maintenance;

    public LocalizationMaintenanceController(LocalizationMaintenanceService maintenance)
    {
        _maintenance = maintenance;
    }

    [HttpPost("repair")]
    public async Task<IActionResult> Repair([FromQuery] string? tenantId = null)
    {
        await _maintenance.RepairMissingValuesAsync(tenantId);
        return Ok(new { Status = "Completed" });
    }
}
