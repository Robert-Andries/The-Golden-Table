using GoldenTable.Modules.Order.Domain.Dishes;
using GoldenTable.Modules.Order.Domain.Dishes.ValueObjects;

namespace GoldenTable.Modules.Order.Domain.Orders.ValueObjects;

public sealed record DishItem(Guid DishId, DishSize SelectedSize, int Quantity);
