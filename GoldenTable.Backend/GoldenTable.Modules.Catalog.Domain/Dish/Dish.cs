using System.Drawing;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dish.Enums;
using GoldenTable.Modules.Catalog.Domain.Dish.Events;
using GoldenTable.Modules.Catalog.Domain.Dish.ValueObject;

namespace GoldenTable.Modules.Catalog.Domain.Dish;

public sealed class Dish : Entity
{
    private Dish()
    { }
    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime ModifiedOnUtc { get; private set; }
    public Money BasePrice { get; private set; }
    public List<Size> Sizes { get; private init; }
    public NutritionalValues NutritionalInformation { get; private set; }
    public List<Guid> ImagesIds { get; private init; }
    public DishCategory Category { get; private set; }
    public List<DishTag> Tags { get; private init; }
    
    public static Result<Dish> Create(string name, string description, Money basePrice, List<Size> sizes,
        NutritionalValues nutritionalInformation, List<Guid> imagesIds, DishCategory category, List<DishTag> tags,
        DateTime nowUtc)

    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<Dish>(DishErrors.InvalidName);
        }

        if (string.IsNullOrEmpty(description))
        {
            return Result.Failure<Dish>(DishErrors.InvalidDescription);
        }

        var dish = new Dish
        {
            Id = Guid.NewGuid(), 
            Name = new(name),
            Description = new(description),
            CreatedOnUtc = nowUtc,
            ModifiedOnUtc = nowUtc,
            BasePrice = basePrice,
            Sizes = sizes,
            NutritionalInformation = nutritionalInformation,
            ImagesIds = imagesIds,
            Category = category,
            Tags = tags
        };

        dish.Raise(new DishCreatedDomainEvent(dish.Id, nowUtc));
        return dish;
    }

    public Result Rename(string name, DateTime nowUtc)
    {
        if (Name.Value == name)
        {
            return DishErrors.SameName;
        }

        Name = new(name);
        ModifiedOnUtc = nowUtc;
        Raise(new DishRenamedDomainEvent(Id, Name, nowUtc));
        return Result.Success();
    }

    public Result UpdateDescription(string description, DateTime nowUtc)
    {
        if (Description.Value == description)
        {
            return DishErrors.SameDescription;
        }

        Description = new(description);
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedDescriptionDomainEvent(Id, Description,  nowUtc));
        return Result.Success();
    }

    public Result UpdateBasePrice(Money newPrice, DateTime nowUtc)
    {
        if (BasePrice == newPrice)
        {
            return DishErrors.SamePrice;
        }

        BasePrice = newPrice;
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedPriceDomainEvent(Id, BasePrice, nowUtc));
        return Result.Success();
    }

    public Result AddSize(Size size, DateTime nowUtc)
    {
        if (Sizes.Contains(size))
        {
            return DishErrors.SizeAlreadyPresent;
        }

        Sizes.Add(size);
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedSizeDomainEvent( Id, nowUtc));
        return Result.Success();
    }
    
    public Result RemoveSize(Size size, DateTime nowUtc)
    {
        if (!Sizes.Contains(size))
        {
            return DishErrors.SizeDoesNotExist;
        }

        Sizes.Remove(size);
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedSizeDomainEvent(Id, nowUtc));
        return Result.Success();
    }

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

    public Result AddImage(Guid imageId, DateTime nowUtc)
    {
        if (ImagesIds.Contains(imageId))
        {
            return DishErrors.ImageAlreadyPresent;
        }
        ImagesIds.Add(imageId);
        Raise(new DishUpdatedImagesDomainEvent(Id, nowUtc));
        return Result.Success();
    }

    public Result RemoveImage(Guid imageId, DateTime nowUtc)
    {
        if (!ImagesIds.Contains(imageId))
        {
            return DishErrors.ImageNotPresent;
        }
        ImagesIds.Remove(imageId);
        Raise(new DishUpdatedImagesDomainEvent(Id, nowUtc));
        return Result.Success();
    }

    public Result UpdateDishCategory(DishCategory category, DateTime nowUtc)
    {
        Category = category;
        Raise(new DishUpdatedCategoryDomainEvent(Id, nowUtc));
        return Result.Success();
    }

    public Result AddTags(List<DishTag> tags, DateTime nowUtc)
    {
        if (tags.Count == 0)
        {
            return DishErrors.InvalidTags;
        }

        var uniqueTags = tags.Where(t => !Tags.Contains(t)).ToList();
        foreach (DishTag uniqueTag in uniqueTags)
        {
            Tags.Add(uniqueTag);
        }
        Raise(new DishUpdatedTagsDomainEvent(Id, nowUtc));
        return Result.Success();
    }
    
    public Result RemoveTags(List<DishTag> tags, DateTime nowUtc)
    {
        if (tags.Count == 0)
        {
            return DishErrors.InvalidTags;
        }

        var uniqueTags = tags.Where(t => !Tags.Contains(t)).ToList();
        foreach (DishTag uniqueTag in uniqueTags)
        {
            Tags.Remove(uniqueTag);
        }
        Raise(new DishUpdatedTagsDomainEvent(Id, nowUtc));
        return Result.Success();
    }
}
