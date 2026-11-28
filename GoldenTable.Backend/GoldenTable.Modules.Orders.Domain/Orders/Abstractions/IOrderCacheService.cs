namespace GoldenTable.Modules.Orders.Domain.Orders.Abstractions;

/// <summary>
///     A simple way to interact with the order cache
/// </summary>
public interface IOrderCacheService
{
    /// <summary>
    ///     Gets the cache entry for the order with the provided id
    /// </summary>
    /// <param name="orderId">Id to search for</param>
    /// <returns>Order object with the specified id, null if not found</returns>
    Task<Order?> GetAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates the cache entry of an order
    /// </summary>
    /// <param name="order">The order to update</param>
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes the cache entry of an order
    /// </summary>
    /// <param name="orderId">The order id to remove</param>
    Task RemoveAsync(Guid orderId, CancellationToken cancellationToken = default);
}
