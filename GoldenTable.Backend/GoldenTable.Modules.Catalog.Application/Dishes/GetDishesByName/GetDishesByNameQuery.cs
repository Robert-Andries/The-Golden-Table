using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByName;

public sealed record GetDishesByNameQuery(Name Name) : IQuery<List<DishResponse>>;
