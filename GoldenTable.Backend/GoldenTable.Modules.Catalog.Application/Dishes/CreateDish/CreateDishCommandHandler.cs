using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;

public sealed class CreateDishCommandHandler(
    IUnitOfWork unitOfWork,
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    ILogger<CreateDishCommandHandler> logger,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateDishCommand, Dish>
{
    public async Task<Result<Dish>> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Result<Dish> result = Dish.Create(
            request.Name,
            request.Description,
            request.BasePrice,
            request.Sizes,
            request.NutritionalInformation,
            request.ImageIds,
            request.DishCategory,
            request.Tags,
            dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            logger.LogInformation("Attempted to create a dish but failed. Error: {Error}", result.Error);
            return Result.Failure<Dish>(result.Error);
        }

        Dish dish = result.Value;
        
        await dishRepository.AddAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.CrateAsync(dish, cancellationToken);
        
        logger.LogInformation("Dish created successfully");
        return Result.Success(dish);
    }
}
