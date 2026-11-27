using GoldenTable.Common.Application.Caching;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;

namespace GoldenTable.Modules.Orders.Infrastructure.Dishes;

internal sealed class DishCacheService(ICacheService cacheService) : IDishCacheService
{
    private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(30);

    public Task<Dish?> GetAsync(Guid dishId, CancellationToken cancellationToken = default)
    {
        return cacheService.GetAsync<Dish>(GetCacheKey(dishId), cancellationToken);
    }

    public Task UpdateAsync(Dish dish, CancellationToken cancellationToken = default)
    {
        return cacheService.SetAsync(GetCacheKey(dish.Id), dish, ExpirationTime, cancellationToken);
    }

    public Task RemoveAsync(Guid dishId, CancellationToken cancellationToken = default)
    {
        return cacheService.RemoveAsync(GetCacheKey(dishId), cancellationToken);
    }

    // Prefixed with the module: Catalog caches its own Dish type under "Dish-{id}" in the same Redis
    private static string GetCacheKey(Guid dishId)
    {
        return $"Orders-Dish-{dishId}";
    }
}
