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
}
