using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.EditDish;

public sealed class EditDishCommandHandler(
    IUnitOfWork unitOfWork,
    IDishDbSets dishDbSets,
    IDishRepository dishRepository,
    IDishTagRepository dishTagRepository,
    IImageRepository imageRepository,
    IImageCacheService imageCacheService,
    IDishCacheService dishCacheService,
    ILogger<EditDishCommandHandler> logger,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<EditDishCommand>
{
    public async Task<Result> Handle(EditDishCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Dish? dish = await dishRepository.GetAsync(request.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, request.DishId);
            return Result.Failure(DishErrors.NotFound);
        }

        if (dish.Name.Value != request.Name && dishDbSets.Dishes.Any(d => d.Name.Value == request.Name))
        {
            return Result.Failure(DishErrors.DishAlreadyExists);
        }

        DateTime now = dateTimeProvider.UtcNow;

        Name name = new(request.Name);
        Result renameResult = dish.Rename(name, now);
        if (renameResult.IsFailure && renameResult.Error != DishErrors.SameName) 
        {
            return renameResult;
        }

        Description description = new(request.Description);
        Result descResult = dish.UpdateDescription(description, now);
        if (descResult.IsFailure && descResult.Error != DishErrors.SameDescription) 
        {
            return descResult;
        }

        Result<Money> basePriceResult = Money.Create(request.BasePriceAmount, request.BasePriceCurrency);
        if (basePriceResult.IsFailure) 
        {
            return basePriceResult;
        }

        Result priceResult = dish.UpdateBasePrice(basePriceResult.Value, now);
        if (priceResult.IsFailure && priceResult.Error != DishErrors.SamePrice) 
        {
            return priceResult;
        }

        Result<NutritionalValues> nutritionalInformationResult = NutritionalValues.Create(
            request.Kcal,
            request.GramsOfFat,
            request.GramsOfCarbohydrates,
            request.GramsOfSugar,
            request.GramsOfProtein,
            request.GramsOfSalt);
        if (nutritionalInformationResult.IsFailure) 
        {
            return nutritionalInformationResult;
        }

        Result nutResult = dish.UpdateNutritionalInformation(nutritionalInformationResult.Value, now);
        if (nutResult.IsFailure && nutResult.Error != DishErrors.NutritionalInformationIsTheSame) 
        {
            return nutResult;
        }

        Result<DishCategory> dishCategoryResult = DishCategory.Create(request.DishCategory);
        if (dishCategoryResult.IsFailure) 
        {
            return dishCategoryResult;
        }

        Result catResult = dish.UpdateDishCategory(dishCategoryResult.Value, now);
        if (catResult.IsFailure && catResult.Error != DishErrors.SameCategory) 
        {
            return catResult;
        }

        // Update Sizes
        var newSizesDict = request.Sizes.ToDictionary(s => s.Name);
        var sizesToRemove = dish.Sizes.Where(s => !newSizesDict.ContainsKey(s.Name) || newSizesDict[s.Name] != s).ToList();
        foreach (DishSize s in sizesToRemove)
        {
            Result removeSizeResult = dish.RemoveSize(s.Name, now);
            if (removeSizeResult.IsFailure) 
            {
                return removeSizeResult;
            }
        }
        var sizesToAdd = request.Sizes.Where(s => !dish.Sizes.Any(ds => ds.Name == s.Name)).ToList();
        foreach (DishSize s in sizesToAdd)
        {
            Result addSizeResult = dish.AddSize(s, now);
            if (addSizeResult.IsFailure) 
            {
                return addSizeResult;
            }
        }

        // Update Tags
        var currentTagIds = dish.Tags.Select(t => t.Id).ToHashSet();
        var newTagIds = request.TagIds.ToHashSet();

        var tagsToRemove = dish.Tags.Where(t => !newTagIds.Contains(t.Id)).ToList();
        if (tagsToRemove.Any())
        {
            Result removeTagsResult = dish.RemoveTags(tagsToRemove, now);
            if (removeTagsResult.IsFailure) 
            {
                return removeTagsResult;
            }
        }

        var tagIdsToAdd = newTagIds.Except(currentTagIds).ToList();
        if (tagIdsToAdd.Any())
        {
            List<DishTag> tagsToAdd = await dishTagRepository.GetByIdsAsync(tagIdsToAdd, cancellationToken);
            if (tagsToAdd.Count != tagIdsToAdd.Count) 
            {
                return Result.Failure(DishTagErrors.SomeTagsNotFound);
            }
            
            Result addTagsResult = dish.AddTags(tagsToAdd, now);
            if (addTagsResult.IsFailure) 
            {
                return addTagsResult;
            }
        }

        // Update Images
        var currentImageIds = dish.Images.Select(i => i.Id).ToHashSet();
        var newImageIds = request.ImageIds.ToHashSet();

        var imageIdsToRemove = currentImageIds.Except(newImageIds).ToList();
        foreach (Guid id in imageIdsToRemove)
        {
            Result removeImageResult = dish.RemoveImage(id, now);
            if (removeImageResult.IsFailure) 
            {
                return removeImageResult;
            }
        }

        var imageIdsToAdd = newImageIds.Except(currentImageIds).ToList();
        foreach (Guid id in imageIdsToAdd)
        {
            Image? image = await imageCacheService.GetAsync(id, cancellationToken) ?? await imageRepository.GetAsync(id, cancellationToken);
            if (image is null) 
            {
                return Result.Failure(ImageErrors.NotFound);
            }
            
            Result addImageResult = dish.AddImage(image, now);
            if (addImageResult.IsFailure) 
            {
                return addImageResult;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);

        return Result.Success();
    }
}
