using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishById;

public sealed class GetDishByIdQueryHandler(
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    ILogger<GetDishByIdQueryHandler> logger)
    : IQueryHandler<GetDishByIdQuery, DishResponse>
{
    public async Task<Result<DishResponse>> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Dish? dish = await dishCacheService.GetAsync(request.DishId, cancellationToken) ?? 
                     await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, request.DishId);
            return Result.Failure<DishResponse>(DishErrors.NotFound);
        }

        return new DishResponse(dish);
    }
}
