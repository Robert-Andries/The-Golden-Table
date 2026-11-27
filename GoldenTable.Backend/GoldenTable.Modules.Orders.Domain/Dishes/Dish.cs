using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Orders.Domain.Dishes.Events;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using Name = GoldenTable.Common.Domain.Name;

namespace GoldenTable.Modules.Orders.Domain.Dishes;

public sealed class Dish : Entity
{
    private List<DishSize> _sizes = new();

    private Dish()
    {
    }

    /// <summary> The dish name </summary>
    public Name Name { get; private set; }

    /// <summary> The dish base price </summary>
    public Money BasePrice { get; private set; }

    /// <summary>When was the dish created</summary>
    public DateTime CreatedOnUtc { get; private set; }

    /// <summary>When was the dish last modified</summary>
    public DateTime ModifiedOnUtc { get; private set; }

    /// <summary>The sizes of a dish (e.g. small, medium, large)</summary>
    public IReadOnlyCollection<DishSize> Sizes => _sizes.AsReadOnly();

    /// <summary>
    ///     Factory method to create a dish
    /// </summary>
    public static Result<Dish> Create(Name name, Money basePrice, DateTime nowUtc, List<DishSize> sizes)
    {
        return Create(Guid.NewGuid(), name, basePrice, nowUtc, sizes);
    }

    /// <summary>
    ///     Factory method to create a dish with a known id (e.g. a copy of a dish owned by another module)
    /// </summary>
    public static Result<Dish> Create(Guid id, Name name, Money basePrice, DateTime nowUtc, List<DishSize> sizes)
    {
        if (!name.IsValid())
        {
            return Result.Failure<Dish>(DishErrors.InvalidName);
        }

        var dish = new Dish
        {
            Id = id,
            Name = name,
            BasePrice = basePrice,
            CreatedOnUtc = nowUtc,
            ModifiedOnUtc = nowUtc,
            _sizes = sizes
        };
        dish.Raise(new DishCreatedDomainEvent(dish.Id));
        return Result.Success(dish);
    }

    /// <summary>
    ///     Method to update the name of the dish
    /// </summary>
    /// <param name="name">The new name</param>
    /// <param name="nowUtc">When the action was performed</param>
    public Result UpdateName(Name name, DateTime nowUtc)
    {
        if (!name.IsValid())
        {
            return Result.Failure(DishErrors.InvalidName);
        }

        if (Name == name)
        {
            return Result.Failure(DishErrors.SameName);
        }

        Name = name;
        ModifiedOnUtc = nowUtc;
        Raise(new DishRenamedDomainEvent(Id, name));
        return Result.Success();
    }

    /// <summary>
    ///     Method used to update the BasePrice property
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
        Raise(new DishUpdatedPriceDomainEvent(Id, newPrice.Amount, newPrice.Currency.Code));
        return Result.Success();
    }

    /// <summary>
    ///     Assigns the provided sizes to the dish, removing the previous sizes
    /// </summary>
    /// <param name="sizes">The sizes to assign</param>
    /// <param name="nowUtc">When the operation occured</param>
    /// <returns>Result indicating success and the error that occured</returns>
    public Result UpdateSizes(IReadOnlyList<DishSize> sizes, DateTime nowUtc)
    {
        if (sizes.Any(s => string.IsNullOrEmpty(s.Name) || s.Weight <= 0))
        {
            return DishErrors.InvalidSize;
        }

        if (sizes.DistinctBy(s => s.Name).Count() != sizes.Count)
        {
            return DishErrors.SizeAlreadyPresent;
        }

        if (_sizes.OrderBy(s => s.Name).SequenceEqual(sizes.OrderBy(s => s.Name)))
        {
            return Result.Success();
        }

        _sizes.Clear();
        _sizes.AddRange(sizes);
        ModifiedOnUtc = nowUtc;
        Raise(new DishUpdatedSizesDomainEvent(Id, _sizes.ToList()));
        return Result.Success();
    }
}
