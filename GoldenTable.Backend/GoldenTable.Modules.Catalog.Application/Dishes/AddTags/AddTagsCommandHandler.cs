using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Dishes.AddSize;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.AddTags;

public sealed class AddTagsCommandHandler(
    IUnitOfWork unitOfWork,
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    IDateTimeProvider dateTimeProvider,
    ILogger<AddTagsCommandHandler> logger)
    : ICommandHandler<AddTagsCommand>
{
    public async Task<Result> Handle(AddTagsCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Dish? dish = await dishCacheService.GetAsync(request.DishId, cancellationToken) ?? 
                     await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            logger.LogInformation("Dish {DishId} not found", request.DishId);
            return DishErrors.NotFound;
        }
        
        Result result = dish.AddTags(request.Tags, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            logger.LogInformation("Could not add tags to dish with id: {DishId}. Error: {Error}",
                request.DishId, result.Error);
            return Result.Failure(result.Error);
        }
        
        await dishRepository.UpdateAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);

        return Result.Success();
    }
}
