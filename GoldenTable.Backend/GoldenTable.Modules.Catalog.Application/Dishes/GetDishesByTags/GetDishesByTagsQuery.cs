using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByTags;

public sealed record GetDishesByTagsQuery(List<Guid> TagIds) : IQuery<List<DishResponse>>;
