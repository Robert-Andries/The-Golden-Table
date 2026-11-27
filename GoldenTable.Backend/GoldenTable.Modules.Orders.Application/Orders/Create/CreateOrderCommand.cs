using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Application.Orders.Create;

public sealed record CreateOrderCommand(
    Guid CustomerId,
    IReadOnlyCollection<(Guid id, string? itemSizeName, int quantity)> Items,
    Address Address) : ICommand<Order>;
