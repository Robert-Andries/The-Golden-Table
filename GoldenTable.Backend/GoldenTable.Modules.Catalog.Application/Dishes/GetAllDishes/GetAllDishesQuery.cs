using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetAllDishes;

public sealed record GetAllDishesQuery() : IQuery<List<Dish>>;
