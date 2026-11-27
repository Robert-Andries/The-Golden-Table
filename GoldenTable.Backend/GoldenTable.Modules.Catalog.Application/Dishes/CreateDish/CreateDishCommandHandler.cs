using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;

public sealed class CreateDishCommandHandler(
    IUnitOfWork unitOfWork,
    IDishDbSets  dishDbSets,
    IDishRepository dishRepository,
    IDishTagRepository dishTagRepository,
    IDishCacheService dishCacheService,
    ILogger<CreateDishCommandHandler> logger,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateDishCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (dishDbSets.Dishes.Any(d => d.Name.Value == request.Name))
        {
            return Result.Failure<Guid>(DishErrors.DishAlreadyExists);
        }

        List<DishTag> tags = await dishTagRepository.GetByIdsAsync(request.TagIds, cancellationToken);
        if (tags.Count != request.TagIds.Count)
        {
            return Result.Failure<Guid>(DishTagErrors.SomeTagsNotFound);
        }
        
        Name name = new(request.Name);
        Description description = new(request.Description);
        Result<Money> basePriceResult = Money.Create(request.BasePriceAmount, request.BasePriceCurrency);
        if (basePriceResult.IsFailure)
        {
            return Result.Failure<Guid>(basePriceResult.Error);
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
            return Result.Failure<Guid>(nutritionalInformationResult.Error);
        }

        NutritionalValues nutritionalInformation = nutritionalInformationResult.Value;
        Result<DishCategory> dishCategoryResult = DishCategory.Create(request.DishCategory);
        if (dishCategoryResult.IsFailure)
        {
            DishLogs.CreateCategoryError(logger, dishCategoryResult.Error);
            return Result.Failure<Guid>(dishCategoryResult.Error);
        }

        DishCategory dishCategory = dishCategoryResult.Value;

        Result<Dish> result = Dish.Create(
            name,
            description,
            basePrice,
            request.Sizes,
            nutritionalInformation,
            dishCategory,
            tags,
            dateTimeProvider.UtcNow);

        if (result.IsFailure)
        {
            DishLogs.CreateError(logger, result.Error);
            return Result.Failure<Guid>(result.Error);
        }

        Dish dish = result.Value;

        await dishRepository.AddAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);

        DishLogs.DishCreatedSuccessfully(logger, dish.Id);
        return Result.Success(dish.Id);
    }
}
