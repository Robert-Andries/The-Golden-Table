namespace GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

public sealed record DishItem(Guid DishId, string? SelectedSizeName, int Quantity);
