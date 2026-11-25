using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Orders.Application.Orders.RequestCancel;

public sealed record RequestCancelCommand(Guid OrderId) : ICommand;
