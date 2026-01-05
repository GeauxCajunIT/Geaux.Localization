using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Geaux.Localization.Services.Maintenance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("admin/localization/api/cultures")]
public sealed class LocalizationCulturesController : ControllerBase
{
    private readonly GeauxLocalizationDbContext _db;
    private readonly LocalizationMaintenanceService _maintenance;

    public LocalizationCulturesController(
        GeauxLocalizationDbContext db,
        LocalizationMaintenanceService maintenance)
    {
        _db = db;
        _maintenance = maintenance;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCulture([FromBody] LocalizationCulture model)
    {
        if (string.IsNullOrWhiteSpace(model.CultureCode))
            return BadRequest("CultureCode is required.");

        _db.LocalizationCultures.Add(model);
        await _db.SaveChangesAsync();

        // Seed values for all existing keys for this culture
        await _maintenance.SeedValuesForCultureAsync(model.CultureCode);

        return CreatedAtAction(nameof(GetCultureById), new { id = model.Id }, model);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocalizationCulture>> GetCultureById(int id)
    {
        LocalizationCulture? culture = await _db.LocalizationCultures.FindAsync(id);
        if (culture is null) return NotFound();
        return culture;
    }

    [HttpGet]
    public async Task<ActionResult<List<LocalizationCulture>>> GetAll()
        => await _db.LocalizationCultures.AsNoTracking().ToListAsync();
}

