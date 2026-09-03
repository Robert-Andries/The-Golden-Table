using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Tag;

/// <summary>
///     Entity holding the data for a dish tag
/// </summary>
public sealed class DishTag : Entity
{
    private DishTag()
    {
    }

    /// <summary>
    ///     The actual dish tag
    /// </summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>
    ///     Factory method to create a dish tag
    /// </summary>
    /// <param name="value">The dish tag</param>
    /// <returns>Result indicating success, the error that occured and the newly created object</returns>
    public static Result<DishTag> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Result.Failure<DishTag>(DishTagErrors.InvalidValue);
        }

        return new DishTag
        {
            Id = Guid.NewGuid(),
            Value = value
        };
    }

    /// <summary>
    ///     Updates the value of the tag
    /// </summary>
    /// <param name="newValue">The new value for the tag</param>
    /// <returns>Result indicating success or the error that occurred</returns>
    public Result UpdateValue(string newValue)
    {
        if (string.IsNullOrEmpty(newValue))
        {
            return Result.Failure(DishTagErrors.InvalidValue);
        }

        if (Value == newValue)
        {
            return Result.Failure(DishTagErrors.SameValue);
        }

        Value = newValue;
        return Result.Success();
    }
}
