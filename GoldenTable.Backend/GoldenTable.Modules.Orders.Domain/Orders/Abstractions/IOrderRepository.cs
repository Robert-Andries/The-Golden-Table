namespace GoldenTable.Modules.Orders.Domain.Orders.Abstractions;

/// <summary>
///     Repository for orders
/// </summary>
/// <remarks>No changes will be persisted on the repository until saving is called</remarks>
public interface IOrderRepository
{
    /// <summary>
    ///     Adds an order to the repository
    /// </summary>
    /// <param name="order">Order to add</param>
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the order with the specified id from the repository
    /// </summary>
    /// <param name="orderId">The id to search for</param>
    /// <returns>Order object if found, null otherwise</returns>
    Task<Order?> GetAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all the orders
    /// </summary>
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the orders placed by a customer
    /// </summary>
    /// <param name="customerId">The customer who placed the orders</param>
    Task<List<Order>> GetByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates the order from the repository based on id.
    /// </summary>
    /// <param name="order">Order to update</param>
    /// <remarks>If the provided dish does not exist in the repository, the method will simply return</remarks>
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes the order from the repository
    /// </summary>
    /// <param name="order">The order to remove</param>
    void Remove(Order order);
}
