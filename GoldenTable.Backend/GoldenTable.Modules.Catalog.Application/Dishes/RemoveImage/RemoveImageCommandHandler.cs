using GoldenTable.Common.Application.Caching;
using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.RemoveImage;

public sealed class RemoveImageCommandHandler(
    ILogger<RemoveImageCommandHandler> logger,
    IDishRepository dishRepository,
    IUnitOfWork unitOfWork,
    IDishCacheService cacheService,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<RemoveImageCommand>
{
    public async Task<Result> Handle(RemoveImageCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Dish? dish = await cacheService.GetAsync(request.DishId, cancellationToken) ?? 
                     await dishRepository.GetAsync(request.DishId, cancellationToken);

        if (dish is null)
        {
            logger.LogInformation("Dish with id: {DishId} not found", request.DishId);
            return DishErrors.NotFound;
        }
        
        Result result = dish.RemoveImage(request.ImageId, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            logger.LogInformation("There was a problem removing an image with id: {ImageId} for a dish with id: {DishId}" +
                                  "Error: {Error}",request.ImageId, request.DishId, result.Error);
            return result.Error;
        }
        
        await dishRepository.UpdateAsync(dish, cancellationToken);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cacheService.UpdateAsync(dish, cancellationToken);

        return Result.Success();
    }
}
