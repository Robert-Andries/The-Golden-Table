using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Orders.Domain.Orders;

namespace GoldenTable.Modules.Orders.Application.Orders.GetOrders;

public sealed class GetOrdersQuery : IQuery<IReadOnlyCollection<Order>>;
