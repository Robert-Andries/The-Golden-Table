using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishById;

public sealed record GetDishByIdQuery(Guid DishId) : IQuery<Dish>;
