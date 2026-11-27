using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByName;

public sealed record GetDishesByNameQuery(Name Name) : IQuery<List<DishResponse>>;
