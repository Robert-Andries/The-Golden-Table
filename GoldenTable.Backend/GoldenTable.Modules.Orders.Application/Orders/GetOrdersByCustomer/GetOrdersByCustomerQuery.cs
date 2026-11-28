using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Orders.Domain.Orders;

namespace GoldenTable.Modules.Orders.Application.Orders.GetOrdersByCustomer;

public sealed record GetOrdersByCustomerQuery(Guid CustomerId) : IQuery<IReadOnlyCollection<Order>>;
