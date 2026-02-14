using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Dishes;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByName;

public sealed record GetDishesByNameQuery(Name Name) : IQuery<List<Dish>>;
