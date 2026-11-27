using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishById;

public sealed class GetDishByIdQueryHandler(
    IDishDbSets dishDbSets,
    IDishCacheService dishCacheService,
    ILogger<GetDishByIdQueryHandler> logger)
    : IQueryHandler<GetDishByIdQuery, DishResponse>
{
    public async Task<Result<DishResponse>> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Dish? dish = await dishCacheService.GetAsync(request.DishId, cancellationToken) ??
                     await dishDbSets.Dishes
                         .AsNoTracking()
                         .Include(d => d.Images)
                         .Include(d => d.Tags)
                         .FirstOrDefaultAsync(d => d.Id == request.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, request.DishId);
            return Result.Failure<DishResponse>(DishErrors.NotFound);
        }

        return new DishResponse(dish);
    }
}
