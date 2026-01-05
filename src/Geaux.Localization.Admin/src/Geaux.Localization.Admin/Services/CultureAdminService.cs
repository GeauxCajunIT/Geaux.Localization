using Geaux.Localization.Admin.DTOs;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Geaux.Shared.Activity;
using Microsoft.EntityFrameworkCore;

namespace Geaux.Localization.Admin.Services;

/// <summary>
/// Provides administrative operations for managing localization cultures.
/// This includes retrieving cultures, updating culture metadata, and
/// toggling activation or review flags. All operations return DTOs to
/// maintain a clean separation between persistence and presentation layers.
/// </summary>
public sealed class CultureAdminService
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _factory;
    private readonly IActivityLogRepository _activity;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureAdminService"/>.
    /// </summary>
    /// <param name="factory">The database context factory.</param>
    /// <param name="activity">The activity log repository.</param>
    public CultureAdminService(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IActivityLogRepository activity)
    {
        _factory = factory;
        _activity = activity;
    }

    /// <summary>
    /// Retrieves all localization cultures in the system.
    /// </summary>
    /// <returns>A list of <see cref="LocalizationCultureDto"/>.</returns>
    public async Task<List<LocalizationCultureDto>> GetAllCulturesAsync()
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        return await db.LocalizationCultures
            .OrderBy(c => c.DisplayName)
            .Select(c => new LocalizationCultureDto
            {
                Id = c.Id,
                CultureCode = c.CultureCode,
                DisplayName = c.DisplayName,
                IsActive = c.IsActive,
                IsRightToLeft = c.IsRightToLeft
            })
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a single culture by its identifier.
    /// </summary>
    /// <param name="id">The culture identifier.</param>
    /// <returns>A <see cref="LocalizationCultureDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the culture does not exist.</exception>
    public async Task<LocalizationCultureDto> GetCultureAsync(int id)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationCulture? entity = await db.LocalizationCultures.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"Localization culture {id} was not found.");

        return new LocalizationCultureDto
        {
            Id = entity.Id,
            CultureCode = entity.CultureCode,
            DisplayName = entity.DisplayName,
            IsActive = entity.IsActive,
            IsRightToLeft = entity.IsRightToLeft
        };
    }

    /// <summary>
    /// Creates a new localization culture.
    /// </summary>
    /// <param name="dto">The culture DTO.</param>
    /// <returns>The ID of the created culture.</returns>
    public async Task<int> CreateCultureAsync(LocalizationCultureDto dto)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationCulture entity = new LocalizationCulture
        {
            CultureCode = dto.CultureCode,
            DisplayName = dto.DisplayName,
            IsActive = dto.IsActive,
            IsRightToLeft = dto.IsRightToLeft
        };

        db.LocalizationCultures.Add(entity);
        await db.SaveChangesAsync();

        await _activity.AddAsync(
            ActivityLogFactory.CultureCreated(dto.CultureCode)
        );

        return entity.Id;
    }

    /// <summary>
    /// Updates an existing localization culture.
    /// </summary>
    /// <param name="dto">The updated culture DTO.</param>
    public async Task UpdateCultureAsync(LocalizationCultureDto dto)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationCulture? entity = await db.LocalizationCultures.FirstOrDefaultAsync(c => c.Id == dto.Id);
        if (entity is null)
            throw new KeyNotFoundException($"Localization culture {dto.Id} was not found.");

        entity.CultureCode = dto.CultureCode;
        entity.DisplayName = dto.DisplayName;
        entity.IsActive = dto.IsActive;
        entity.IsRightToLeft = dto.IsRightToLeft;

        await db.SaveChangesAsync();

        await _activity.AddAsync(
            ActivityLogFactory.CultureUpdated(dto.CultureCode)
        );
    }

    /// <summary>
    /// Deletes a localization culture.
    /// </summary>
    /// <param name="id">The culture identifier.</param>
    public async Task DeleteCultureAsync(int id)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationCulture? entity = await db.LocalizationCultures.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null)
            return;

        db.LocalizationCultures.Remove(entity);
        await db.SaveChangesAsync();

        await _activity.AddAsync(
            ActivityLogFactory.CultureDeleted(entity.CultureCode)
        );
    }

    /// <summary>
    /// Marks a culture as requiring review.
    /// </summary>
    /// <param name="id">The culture identifier.</param>
    public async Task MarkRequiresReviewAsync(int id)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationCulture? entity = await db.LocalizationCultures.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"Localization culture {id} was not found.");

        entity.RequiresReview = true;
        await db.SaveChangesAsync();

        await _activity.AddAsync(
            ActivityLogFactory.CultureMarkedForReview(entity.CultureCode)
        );
    }

    /// <summary>
    /// Clears the review-required flag for a culture.
    /// </summary>
    /// <param name="id">The culture identifier.</param>
    public async Task ClearReviewFlagAsync(int id)
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        LocalizationCulture? entity = await db.LocalizationCultures.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null)
            throw new KeyNotFoundException($"Localization culture {id} was not found.");

        entity.RequiresReview = false;
        await db.SaveChangesAsync();

        await _activity.AddAsync(
            ActivityLogFactory.CultureReviewCleared(entity.CultureCode)
        );
    }

    public async Task<List<LocalizationCultureDto>> GetActiveCulturesAsync()
    {
        return (await GetAllCulturesAsync())
            .Where(c => c.IsActive)
            .ToList();
    }

}
