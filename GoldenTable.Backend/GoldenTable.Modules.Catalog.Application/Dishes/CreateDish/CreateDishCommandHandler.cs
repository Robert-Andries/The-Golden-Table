using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;
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
        
        Name name = new(request.Name);
        Description description = new(request.Description);
        Result<Money> basePriceResult = Money.Create(request.BasePriceAmount, request.BasePriceCurrency);
        if (basePriceResult.IsFailure)
        {
            return Result.Failure<Dish>(basePriceResult.Error);
        }
        Money basePrice = basePriceResult.Value;
        Result<NutritionalValues> nutritionalInformationResult = NutritionalValues.Create(
            request.Kcal,
            request.GramsOfFat,
            request.GramsOfCarbohydrates,
            request.GramsOfSugar,
            request.GramsOfProtein,
            request.GramsOfSalt);
        if (nutritionalInformationResult.IsFailure)
        {
            return Result.Failure<Dish>(nutritionalInformationResult.Error);
        }
        NutritionalValues nutritionalInformation = nutritionalInformationResult.Value;
        DishCategory dishCategory = new(request.DishCategory);
        
        Result<Dish> result = Dish.Create(
            name,
            description,
            basePrice,
            request.Sizes,
            nutritionalInformation,
            request.ImageIds,
            dishCategory,
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
