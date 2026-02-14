using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Dishes.RemoveSize;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.Rename;

public sealed class RenameCommandHandler(
    ILogger<RenameCommandHandler> logger,
    IDishRepository dishRepository,
    IUnitOfWork unitOfWork,
    IDishCacheService cacheService,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<RenameCommand>
{
    public async Task<Result> Handle(RenameCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Dish? dish = await cacheService.GetAsync(request.DishId, cancellationToken) ??
                     await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            logger.LogInformation("Dish with id: {DishId} not found", request.DishId);
            return DishErrors.NotFound;
        }

        Result result = dish.Rename(request.Name, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            logger.LogInformation("Cannot rename dish with id: {DishId}. Error: {Error}",
                request.DishId, result.Error);
            return result.Error;
        }

        await dishRepository.UpdateAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.CreateOrUpdateAsync(dish, cancellationToken);
        
        return Result.Success();
    }
}
