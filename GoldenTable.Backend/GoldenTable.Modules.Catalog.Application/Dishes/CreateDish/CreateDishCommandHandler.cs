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

public sealed partial class CreateDishCommandHandler(
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
        Result<DishCategory> dishCategoryResult = DishCategory.Create(request.DishCategory);
        if (dishCategoryResult.IsFailure)
        {
            DishLogs.CreateCategoryError(logger, dishCategoryResult.Error);
            return Result.Failure<Dish>(dishCategoryResult.Error);
        }
        DishCategory dishCategory = dishCategoryResult.Value;
        
        Result<Dish> result = Dish.Create(
            name,
            description,
            basePrice,
            request.Sizes,
            nutritionalInformation,
            dishCategory,
            request.Tags,
            dateTimeProvider.UtcNow);
        
        if (result.IsFailure)
        {
            DishLogs.CreateError(logger, result.Error);
            return Result.Failure<Dish>(result.Error);
        }

        Dish dish = result.Value;
        
        await dishRepository.AddAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);
        
        DishLogs.DishCreatedSuccessfully(logger, dish.Id);
        return Result.Success(dish);
    }

    
}
