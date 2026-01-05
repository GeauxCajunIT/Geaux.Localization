namespace Geaux.Localization.Admin.DTOs;

public sealed class DashboardCultureStatsDto
{
    public string CultureCode { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public int TotalValues { get; set; }
    public int MissingValues { get; set; }

    public double CoveragePercent =>
        TotalValues == 0 ? 0 : (double)(TotalValues - MissingValues) / TotalValues * 100;
}
