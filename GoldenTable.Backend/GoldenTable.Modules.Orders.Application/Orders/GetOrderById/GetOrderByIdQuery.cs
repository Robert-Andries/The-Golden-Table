using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Orders.Domain.Orders;

namespace GoldenTable.Modules.Orders.Application.Orders.GetOrderById;

/// <summary>
///     Gets an order by id
/// </summary>
/// <param name="OrderId">The order to get</param>
/// <param name="CustomerId">
///     When set, the order is only returned if this customer placed it. Customer-facing endpoints must set it;
///     only staff endpoints may leave it null.
/// </param>
public sealed record GetOrderByIdQuery(Guid OrderId, Guid? CustomerId) : IQuery<Order>;
