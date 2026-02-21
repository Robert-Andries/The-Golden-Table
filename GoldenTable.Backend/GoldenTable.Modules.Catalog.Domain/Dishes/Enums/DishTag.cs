using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Enums;

public sealed class DishTag : Entity
{
    private DishTag() 
    { }
    public string Value { get; private set; }

    public static Result<DishTag> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Result.Failure<DishTag>(DishErrors.InvalidValueForTag);
        }

        return new DishTag()
        {
            Id = Guid.NewGuid(),
            Value = value
        };
    }
}
