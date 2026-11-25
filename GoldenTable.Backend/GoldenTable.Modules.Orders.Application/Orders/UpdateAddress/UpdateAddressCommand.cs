using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Application.Orders.UpdateAddress;

public sealed record UpdateAddressCommand(Guid CustomerId, Guid OrderId, Address NewAddress) : ICommand;
