using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetAllDishes;

public sealed class GetAllDishesQueryHandler(
    IDishDbSets dishDbSets)
    : IQueryHandler<GetAllDishesQuery, List<DishResponse>>
{
    public async Task<Result<List<DishResponse>>> Handle(GetAllDishesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<Dish> dishes = await dishDbSets.Dishes.ToListAsync(cancellationToken);
        var output = dishes.Select(x => new DishResponse(x)).ToList();
        return output;
    }
}
