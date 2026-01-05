namespace Geaux.Localization.Admin.DTOs;

public sealed class DashboardDto
{
    public int TotalKeys { get; set; }
    public int TotalCultures { get; set; }
    public int ActiveCultures { get; set; }
    public int CulturesRequiringReview { get; set; }

    public List<DashboardCultureStatsDto> CultureStats { get; set; } = new();
    public List<DashboardActivityDto> RecentActivity { get; set; } = new();
}
