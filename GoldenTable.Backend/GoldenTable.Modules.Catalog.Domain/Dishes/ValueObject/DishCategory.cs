using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

public sealed record DishCategory
{
    private DishCategory()
    { }
    public string Name { get; private init; }

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
};
