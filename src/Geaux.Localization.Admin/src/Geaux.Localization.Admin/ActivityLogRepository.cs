using Geaux.Localization.Contexts;
using Geaux.Shared.Activity;
using Microsoft.EntityFrameworkCore;

namespace Geaux.Localization.Admin
{

    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly GeauxLocalizationDbContext _db;

        public ActivityLogRepository(GeauxLocalizationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(ActivityLog log)
        {
            _db.LocalizationActivityLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        public Task<List<Shared.Activity.ActivityLog>> GetRecentAsync(int take)
        {
            return _db.LocalizationActivityLogs
                .OrderByDescending(x => x.TimestampUtc)
                .Take(take)
                .ToListAsync();
        }
    }
}
