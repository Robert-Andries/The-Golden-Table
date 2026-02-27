using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

/// <summary>
///     Value object holding the data necesarry for a dish category
/// </summary>
public record DishCategory
{
    private DishCategory()
    {
    }

    /// <summary>
    ///     The category name
    /// </summary>
    public string Name { get; private init; }

    /// <summary>
    ///     Factory method used to create a DishCategory object
    /// </summary>
    /// <param name="name">The category name</param>
    /// <returns>Result indicating success, the error that occured and the newly created object</returns>
    public static Result<DishCategory> Create(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return Result.Failure<DishCategory>(DishErrors.InvalidCategoryName);
        }

        return new DishCategory
        {
            Name = name
        };
    }
}
