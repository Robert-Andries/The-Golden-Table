using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishById;

public sealed record GetDishByIdQuery(Guid DishId) : IQuery<DishResponse>;
