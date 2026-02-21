using GoldenTable.Common.Application.Caching;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;

namespace GoldenTable.Modules.Catalog.Infrastructure.Dishes;

internal sealed class DishCacheService(ICacheService cacheService) : IDishCacheService
{
    public Task<Dish?> GetAsync(Guid dishId, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetCacheKey(dishId);
        return cacheService.GetAsync<Dish>(cacheKey, cancellationToken);
    }

    public Task UpdateAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        string cacheKey = GetCacheKey(dish.Id);
        return cacheService.SetAsync(cacheKey, dish, ExpirationTime, cancellationToken);
    }

    private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(30); 
    private static string GetCacheKey(Guid dishId)
    {
        return $"Dish-{dishId}";
    }
}
