using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Application.Orders.ChangeStatus;

public sealed record ChangeStatusCommand(Guid OrderId, OrderStatus NewStatus) : ICommand;
