using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Enums;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByTags;

public sealed record GetDishesByTagsQuery(List<DishTag> Tags) : IQuery<List<Dish>>;
