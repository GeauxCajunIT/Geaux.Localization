using Geaux.Localization.Admin.DTOs;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Geaux.Shared.Activity;
using Microsoft.EntityFrameworkCore;

namespace Geaux.Localization.Admin.Services;

/// <summary>
/// Provides administrative operations for managing localization keys.
/// This service exposes read and write operations used by the Admin UI,
/// returning DTOs instead of EF entities to maintain clean separation
/// between persistence and presentation layers.
/// </summary>
public sealed class KeyAdminService
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _factory;
    private readonly IActivityLogRepository _activity;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyAdminService"/>.
    /// </summary>
    /// <param name="factory">The database context factory.</param>
    /// <param name="activity">The activity log repository.</param>
    public KeyAdminService(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IActivityLogRepository activity)
    {
        _factory = factory;
        _activity = activity;
    }

    /// <summary>
    /// Retrieves all localization keys in the system.
    /// </summary>
    /// <returns>A list of <see cref="LocalizationKeyDto"/>.</returns>
    public async Task<List<LocalizationKeyDto>> GetAllKeysAsync()
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        return await db.LocalizationKeys
            .OrderBy(k => k.Key)
            .Select(k => new LocalizationKeyDto
            {
                Id = k.Id,
                Key = k.Key,
                Description = k.Description,
                IsSystem = k.IsSystem
            })
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a single localization key by its identifier.
    /// </summary>
    /// <param name="id">The key identifier.</param>
    /// <returns>A <see cref="LocalizationKeyDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the key does not exist.</exception>
    public async Task<LocalizationKeyDto> GetKeyAsync(int id)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationKey? entity = await db.LocalizationKeys.FirstOrDefaultAsync(k => k.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"Localization key {id} was not found.");

        return new LocalizationKeyDto
        {
            Id = entity.Id,
            Key = entity.Key,
            Description = entity.Description,
            IsSystem = entity.IsSystem
        };
    }

    /// <summary>
    /// Creates a new localization key.
    /// </summary>
    /// <param name="model">The key DTO.</param>
    /// <returns>The ID of the created key.</returns>
    public async Task<int> CreateKeyAsync(LocalizationKeyDto model)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        var entity = new LocalizationKey
        {
            Key = model.Key,
            Description = model.Description,
            IsSystem = model.IsSystem
        };

        db.LocalizationKeys.Add(entity);
        await db.SaveChangesAsync();

        await _activity.AddAsync(ActivityLogFactory.KeyCreated(model.Key));

        return entity.Id;
    }

    /// <summary>
    /// Updates an existing localization key.
    /// </summary>
    /// <param name="model">The updated key DTO.</param>
    public async Task UpdateKeyAsync(LocalizationKeyDto model)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationKey? entity = await db.LocalizationKeys.FirstOrDefaultAsync(k => k.Id == model.Id);
        if (entity is null)
            throw new KeyNotFoundException($"Localization key {model.Id} was not found.");

        entity.Key = model.Key;
        entity.Description = model.Description;

        await db.SaveChangesAsync();

        await _activity.AddAsync(ActivityLogFactory.KeyUpdated(model.Key));
    }

    /// <summary>
    /// Deletes a localization key.
    /// </summary>
    /// <param name="id">The key identifier.</param>
    public async Task DeleteKeyAsync(int id)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationKey? entity = await db.LocalizationKeys.FirstOrDefaultAsync(k => k.Id == id);
        if (entity is null)
            return;

        db.LocalizationKeys.Remove(entity);
        await db.SaveChangesAsync();

        await _activity.AddAsync(ActivityLogFactory.KeyDeleted(entity.Key));
    }
}
