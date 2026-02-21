using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetAllDishes;

public sealed class GetAllDishesQueryHandler(
    IDishRepository dishRepository)
    : IQueryHandler<GetAllDishesQuery, List<Dish>>
{
    public async Task<Result<List<Dish>>> Handle(GetAllDishesQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        List<Dish> dishes = await dishRepository.GetAllAsync(cancellationToken);
        return dishes;
    }
}
