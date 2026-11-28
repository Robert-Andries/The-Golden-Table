using GoldenTable.Common.Application.Caching;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Abstractions;

namespace GoldenTable.Modules.Orders.Infrastructure.Orders;

internal sealed class OrderCacheService(ICacheService cacheService) : IOrderCacheService
{
    private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(30);

    public Task<Order?> GetAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return cacheService.GetAsync<Order>(GetCacheKey(orderId), cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        return cacheService.SetAsync(GetCacheKey(order.Id), order, ExpirationTime, cancellationToken);
    }

    public Task RemoveAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return cacheService.RemoveAsync(GetCacheKey(orderId), cancellationToken);
    }

    private static string GetCacheKey(Guid orderId)
    {
        return $"Orders-Order-{orderId}";
    }
}
