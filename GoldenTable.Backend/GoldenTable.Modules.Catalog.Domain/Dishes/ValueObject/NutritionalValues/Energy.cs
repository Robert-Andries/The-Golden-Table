using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

/// <summary>
///     Value object holding the nutritional energy data
/// </summary>
public sealed record Energy
{
    private const float KCalToKjValue = (float)4.184;

    private Energy()
    {
    }

    /// <summary>
    ///     Amount of kcal
    /// </summary>
    public float Kcal { get; init; }

    /// <summary>
    ///     Amount of kilojoules
    /// </summary>
    public float Kj => Kcal * KCalToKjValue;

    /// <summary>
    ///     Factory method to create a Energy object
    /// </summary>
    /// <param name="kcal">The amount of kcal</param>
    /// <returns>Result indicating success, the error that occured and the newly created object</returns>
    public static Result<Energy> Create(float kcal)
    {
        if (kcal < 0f)
        {
            return Result.Failure<Energy>(NutritionalValuesErrors.InvalidKcal);
        }

        return new Energy
        {
            Kcal = kcal
        };
    }
}
