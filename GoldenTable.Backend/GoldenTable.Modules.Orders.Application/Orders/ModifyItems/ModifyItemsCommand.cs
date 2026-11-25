using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Orders.Application.Orders.ModifyItems;

public sealed record ModifyItemsCommand(
    Guid OrderId,
    IReadOnlyCollection<(Guid id, string? itemSizeName, int quantity)> Items) : ICommand;
