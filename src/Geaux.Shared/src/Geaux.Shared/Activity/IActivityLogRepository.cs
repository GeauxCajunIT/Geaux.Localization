namespace Geaux.Shared.Activity;

public interface IActivityLogRepository
{
    Task AddAsync(ActivityLog log);
    Task<List<Shared.Activity.ActivityLog>> GetRecentAsync(int take);
}
