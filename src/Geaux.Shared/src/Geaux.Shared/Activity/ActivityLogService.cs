using Geaux.GuardClauses;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Geaux.Shared.Activity;

public class ActivityLogService
{
    private readonly IActivityLogRepository _repo;

    public ActivityLogService(IActivityLogRepository repo)
    {
        _repo = Guard.Against.Null(repo);
    }

    public async Task LogAsync(
        string activityType,
        string description,
        object? metadata = null,
        string? tenantId = null)
    {
        var metadataJson = metadata is null
            ? null
            : JsonSerializer.Serialize(metadata);

        var entry = new ActivityLog(
            activityType,
            description,
            tenantId,
            metadataJson
        );

        await _repo.AddAsync(entry);
    }

    public Task<List<ActivityLog>> GetRecentAsync(int take = 20)
        => _repo.GetRecentAsync(take);
}

