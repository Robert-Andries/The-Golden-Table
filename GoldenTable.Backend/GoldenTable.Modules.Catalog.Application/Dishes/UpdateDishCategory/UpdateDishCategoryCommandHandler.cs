using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateDishCategory;

public sealed partial class UpdateDishCategoryCommandHandler(
    ILogger<UpdateDishCategoryCommandHandler> logger,
    IDishRepository dishRepository,
    IUnitOfWork unitOfWork,
    IDishCacheService cacheService,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateDishCategoryCommand>
{
    public async Task<Result> Handle(UpdateDishCategoryCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Dish? dish = await cacheService.GetAsync(request.DishId, cancellationToken) ??
                     await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, request.DishId);
            return DishErrors.NotFound;
        }

        Result<DishCategory> dishCategoryResult = DishCategory.Create(request.Category);
        if (dishCategoryResult.IsFailure)
        {
            DishLogs.CreateCategoryError(logger, dishCategoryResult.Error);
            return dishCategoryResult.Error;
        }
        DishCategory category = dishCategoryResult.Value;
        
        Result result = dish.UpdateDishCategory(category, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            DishLogs.UpdateCategoryError(logger, request.DishId, result.Error);
            return result.Error;
        }

        await dishRepository.UpdateAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.UpdateAsync(dish, cancellationToken);
        
        return Result.Success();
    }
}
