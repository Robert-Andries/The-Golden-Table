using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetAllDishes;

public sealed record GetAllDishesQuery : IQuery<List<DishResponse>>;
