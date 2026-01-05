using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Geaux.Localization.Services.Maintenance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("admin/localization/api/keys")]
public sealed class LocalizationKeysController : ControllerBase
{
    private readonly GeauxLocalizationDbContext _db;
    private readonly LocalizationMaintenanceService _maintenance;

    public LocalizationKeysController(
        GeauxLocalizationDbContext db,
        LocalizationMaintenanceService maintenance)
    {
        _db = db;
        _maintenance = maintenance;
    }

    [HttpPost]
    public async Task<IActionResult> CreateKey([FromBody] LocalizationKey model)
    {
        if (string.IsNullOrWhiteSpace(model.Key))
            return BadRequest("Key is required.");

        _db.LocalizationKeys.Add(model);
        await _db.SaveChangesAsync();

        // Seed values for this key across all cultures
        await _maintenance.SeedValuesForKeyAsync(model.Id);

        return CreatedAtAction(nameof(GetKeyById), new { id = model.Id }, model);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocalizationKey>> GetKeyById(int id)
    {
        LocalizationKey? key = await _db.LocalizationKeys
            .Include(k => k.Values)
            .FirstOrDefaultAsync(k => k.Id == id);

        if (key is null) return NotFound();
        return key;
    }

    [HttpGet]
    public async Task<ActionResult<List<LocalizationKey>>> GetAll()
        => await _db.LocalizationKeys.AsNoTracking().ToListAsync();
}
