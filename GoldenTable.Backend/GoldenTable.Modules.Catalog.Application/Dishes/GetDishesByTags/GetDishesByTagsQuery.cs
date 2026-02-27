using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByTags;

public sealed record GetDishesByTagsQuery(List<DishTag> Tags) : IQuery<List<DishResponse>>;
