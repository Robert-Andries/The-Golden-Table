using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Domain.Dishes;

/// <summary>
/// An entity representing a dish
/// </summary>
public sealed class Dish : Entity
{
    private Dish()
    {
    }

    /// <summary>The dish name</summary>
    public Name Name { get; private set; }

    /// <summary>The dish description</summary>
    public Description Description { get; private set; }

    /// <summary>When was the dish created</summary>
    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>When was the dish last modified</summary>
    public DateTime ModifiedOnUtc { get; private set; }

    /// <summary>The base price of the dish</summary>
    public Money BasePrice { get; private set; }

    /// <summary>The sizes of a dish (e.g. small, medium, large)</summary>
    public IReadOnlyList<DishSize> Sizes { get; private set; }

    /// <summary>The nutritional values that are needed legally</summary>
    public NutritionalValues NutritionalInformation { get; private set; }

    /// <summary>Images of the dish</summary>
    public IReadOnlyList<Image> Images { get; private set; } = new List<Image>();

    /// <summary>Category of the dish</summary>
    public DishCategory Category { get; private set; }

    /// <summary>Tags of the dish</summary>
    public IReadOnlyList<DishTag> Tags { get; private set; }

    /// <summary>
    /// Factory method used to create a Dish object
    /// </summary>
    /// <returns>Result indicating success, the error that occured and the newly created object</returns>
    public static Result<Dish> Create(Name name, Description description, Money basePrice, List<DishSize> sizes,
        NutritionalValues nutritionalInformation, DishCategory category, List<DishTag> tags,
        DateTime nowUtc)

    {
        if (!name.IsValid())
        {
            return Result.Failure<Dish>(DishErrors.InvalidName);
        }

        if (!description.IsValid())
        {
            return Result.Failure<Dish>(DishErrors.InvalidDescription);
        }

        var dish = new Dish
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedOnUtc = nowUtc,
            ModifiedOnUtc = nowUtc,
            BasePrice = basePrice,
            Sizes = sizes,
            NutritionalInformation = nutritionalInformation,
            Category = category,
            Tags = tags
        };
        dish.Raise(new DishCreatedDomainEvent(dish.Id, nowUtc));
        return dish;
    }

    /// <summary>
    /// Method used to update the Name property
    /// </summary>
    /// <param name="name">The new name</param>
    /// <param name="nowUtc">When the update occures</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result Rename(Name name, DateTime nowUtc)
    {
        if (Name == name)
        {
            return DishErrors.SameName;
        }

        if (!name.IsValid())
        {
            return DishErrors.InvalidName;
        }

        Name = name;
        ModifiedOnUtc = nowUtc;
        Raise(new DishRenamedDomainEvent(Id, Name, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Method used to update the Description property
    /// </summary>
    /// <param name="description">The new description</param>
    /// <param name="nowUtc">When update occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result UpdateDescription(Description description, DateTime nowUtc)
    {
        if (Description == description)
        {
            return DishErrors.SameDescription;
        }

        Description = description;
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedDescriptionDomainEvent(Id, Description, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Method used to update the BasePrice property
    /// </summary>
    /// <param name="newPrice">The new price</param>
    /// <param name="nowUtc">When update occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result UpdateBasePrice(Money newPrice, DateTime nowUtc)
    {
        if (BasePrice == newPrice)
        {
            return DishErrors.SamePrice;
        }

        BasePrice = newPrice;
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedBasePriceDomainEvent(Id, BasePrice, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Method to add a size to the dish
    /// </summary>
    /// <param name="size">Size to add</param>
    /// <param name="nowUtc">When add operation occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result AddSize(DishSize size, DateTime nowUtc)
    {
        if (string.IsNullOrEmpty(size.Name) || size.Weight <= 0)
        {
            return DishErrors.InvalidSize;
        }

        if (Sizes.Any(d => d.Name == size.Name))
        {
            return DishErrors.SizeAlreadyPresent;
        }

        Sizes = [.. Sizes, size];
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedSizeDomainEvent(Id, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Remove a size by its name
    /// </summary>
    /// <param name="sizeName">The name of the size you want to remove</param>
    /// <param name="nowUtc">When remove operation occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result RemoveSize(string sizeName, DateTime nowUtc)
    {
        if (Sizes.All(d => d.Name != sizeName))
        {
            return DishErrors.SizeDoesNotExist;
        }

        DishSize size = Sizes.First(d => d.Name == sizeName);
        Sizes = [.. Sizes.Where(s => s != size)];
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedSizeDomainEvent(Id, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Updates the nutritional information of the dish
    /// </summary>
    /// <param name="nutritionalInformation">The new nutritional information</param>
    /// <param name="nowUtc">When the update occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result UpdateNutritionalInformation(NutritionalValues nutritionalInformation, DateTime nowUtc)
    {
        if (NutritionalInformation == nutritionalInformation)
        {
            return DishErrors.NutritionalInformationIsTheSame;
        }

        NutritionalInformation = nutritionalInformation;
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedNutritionalInformation(Id, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Adds an image to the dish
    /// </summary>
    /// <param name="image">The image to add</param>
    /// <param name="nowUtc">When the operation occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result AddImage(Image image, DateTime nowUtc)
    {
        if (Images.Any(i => i.Id == image.Id))
        {
            return DishErrors.ImageAlreadyPresent;
        }

        Images = [.. Images.ToList(), image];
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedImagesDomainEvent(Id, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Removes an image to the dish by its id
    /// </summary>
    /// <param name="imageId">The id of the image to remove</param>
    /// <param name="nowUtc">When the operation occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result RemoveImage(Guid imageId, DateTime nowUtc)
    {
        if (Images.All(i => i.Id != imageId))
        {
            return DishErrors.ImageNotPresent;
        }

        Images = [.. Images.Where(i => i.Id != imageId)];
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedImagesDomainEvent(Id, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Updates the dish category
    /// </summary>
    /// <param name="category">The new category</param>
    /// <param name="nowUtc">When the operation occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result UpdateDishCategory(DishCategory category, DateTime nowUtc)
    {
        if (Category == category)
        {
            return DishErrors.SameCategory;
        }

        Category = category;
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedCategoryDomainEvent(Id, nowUtc));
        return Result.Success();
    }

    /// <summary>
    /// Adds tags to the tag list
    /// </summary>
    /// <param name="tags">Tags to add</param>
    /// <param name="nowUtc">When the operation occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result AddTags(List<DishTag> tags, DateTime nowUtc)
    {
        if (tags.Count == 0)
        {
            return DishErrors.InvalidTags;
        }

        var uniqueTags = tags.Where(t => !Tags.Contains(t)).ToList();
        if (uniqueTags.Count == 0)
        {
            return DishErrors.TagsAlreadyPresent;
        }

        foreach (DishTag uniqueTag in uniqueTags)
        {
            Tags = [.. Tags, uniqueTag];
        }

        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedTagsDomainEvent(Id, nowUtc));
        return Result.Success();
    }
    
    /// <summary>
    /// Removes tags to the tag list
    /// </summary>
    /// <param name="tags">Tags to remove</param>
    /// <param name="nowUtc">When the operation occured</param>
    /// <returns>Result indicating success and the error that occured </returns>
    public Result RemoveTags(List<DishTag> tags, DateTime nowUtc)
    {
        if (tags.Count == 0)
        {
            return DishErrors.InvalidTags;
        }

        var usefulTags = Tags.IntersectBy(tags.Select(t => t.Id), t => t.Id).ToList();
        if (usefulTags.Count == 0)
        {
            return Result.Success();
        }

        var newTags = Tags.ToList();
        foreach (DishTag uniqueTag in usefulTags)
        {
            newTags.Remove(uniqueTag);
        }

        Tags = newTags;
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedTagsDomainEvent(Id, nowUtc));
        return Result.Success();
    }
}
