using Backend.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Backend.Data.Services;

public class UserPrivilegesCacheService : IUserPrivilegesCacheService
{
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private const string CacheKeyPrefix = "user_privileges_";

    public UserPrivilegesCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<IEnumerable<UserPrivilege>> GetOrSetAsync(Guid userId, Func<Task<IEnumerable<UserPrivilege>>> factory)
    {
        var cacheKey = GetCacheKey(userId);

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await factory();
        }) ?? Enumerable.Empty<UserPrivilege>();
    }

    public void Invalidate(Guid userId)
    {
        var cacheKey = GetCacheKey(userId);
        _cache.Remove(cacheKey);
    }

    private static string GetCacheKey(Guid userId) => $"{CacheKeyPrefix}{userId}";
}
