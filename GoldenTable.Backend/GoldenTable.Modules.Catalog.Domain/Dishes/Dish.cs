using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Domain.Dishes;

public sealed class Dish : Entity
{
    private Dish()
    { }
    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime ModifiedOnUtc { get; private set; }
    public Money BasePrice { get; private set; }
    public IReadOnlyList<DishSize> Sizes { get; private set; }
    public NutritionalValues NutritionalInformation { get; private set; }
    public IReadOnlyList<Image> Images { get; private set; } = new List<Image>();
    public DishCategory Category { get; private set; }
    public IReadOnlyList<DishTag> Tags { get; private set; }
    
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

    public Result UpdateDescription(Description description, DateTime nowUtc)
    {
        if (Description == description)
        {
            return DishErrors.SameDescription;
        }

        Description = description;
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
        Raise(new DishUpdatedBasePriceDomainEvent(Id, BasePrice, nowUtc));
        return Result.Success();
    }

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
        Raise(new DishUpdatedSizeDomainEvent( Id, nowUtc));
        return Result.Success();
    }
    
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
