using Geaux.Localization.Admin.DTOs;
using Geaux.Localization.Contexts;
using Geaux.Localization.Models;
using Geaux.Shared.Activity;
using Microsoft.EntityFrameworkCore;

namespace Geaux.Localization.Admin.Services;

/// <summary>
/// Provides aggregated dashboard data for the localization admin UI.
/// This includes culture statistics, key/value counts, and recent activity.
/// </summary>
public sealed class DashboardService
{
    private readonly IDbContextFactory<GeauxLocalizationDbContext> _factory;
    private readonly IActivityLogRepository _activity;

    public DashboardService(
        IDbContextFactory<GeauxLocalizationDbContext> factory,
        IActivityLogRepository activity)
    {
        _factory = factory;
        _activity = activity;
    }

    /// <summary>
    /// Retrieves all dashboard metrics in a single aggregated DTO.
    /// </summary>
    public async Task<DashboardDto> GetDashboardAsync()
    {
        await using GeauxLocalizationDbContext db = await _factory.CreateDbContextAsync();

        DashboardDto dto = new DashboardDto
        {
            TotalKeys = await db.LocalizationKeys.CountAsync(),
            TotalCultures = await db.LocalizationCultures.CountAsync(),
            ActiveCultures = await db.LocalizationCultures.CountAsync(c => c.IsActive),
            CulturesRequiringReview = await db.LocalizationCultures.CountAsync(c => c.RequiresReview)
        };

        // ------------------------------------------------------------
        // CULTURE COVERAGE
        // ------------------------------------------------------------
        List<LocalizationCulture> cultures = await db.LocalizationCultures
            .OrderBy(c => c.DisplayName)
            .ToListAsync();

        foreach (LocalizationCulture? culture in cultures)
        {
            int totalValues = await db.LocalizationValues
                .CountAsync(v => v.Culture == culture.CultureCode);

            int missingValues = dto.TotalKeys - totalValues;

            dto.CultureStats.Add(new DashboardCultureStatsDto
            {
                CultureCode = culture.CultureCode,
                DisplayName = culture.DisplayName,
                TotalValues = totalValues,
                MissingValues = missingValues
            });
        }

        // ------------------------------------------------------------
        // RECENT ACTIVITY
        // ------------------------------------------------------------
        List<ActivityLog> recent = await _activity.GetRecentAsync(20);

        dto.RecentActivity = recent
            .Select(a => new DashboardActivityDto
            {
                Timestamp = a.TimestampUtc ?? DateTime.UtcNow,      // FIXED
                EventType = a.ActivityType,      // FIXED
                Message = a.Description          // FIXED
            })
            .OrderByDescending(a => a.Timestamp)
            .ToList();

        return dto;
    }
}
