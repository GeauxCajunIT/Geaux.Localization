namespace Geaux.Localization.Admin.DTOs;

public sealed class ActivityLogDto
{
    public DateTime Timestamp { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
}
