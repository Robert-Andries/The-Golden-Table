using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Orders.Domain.Orders;

namespace GoldenTable.Modules.Orders.Application.Orders.GetOrderById;
    
public sealed record GetOrderQuery(Guid OrderId) : IQuery<Order>;
