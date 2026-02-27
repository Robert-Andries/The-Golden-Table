using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

/// <summary>
///     Value object holding the data needed for nutritional carbohydrates
/// </summary>
public sealed record Carbohydrates
{
    private Carbohydrates()
    {
    }

    /// <summary>
    ///     Total grams of carbohydrates
    /// </summary>
    public float Total { get; init; }

    /// <summary>
    ///     The grams of sugar from the total amount
    /// </summary>
    public float OfWhichSugar { get; init; }

    /// <summary>
    ///     Factory method to create a Carbohydrates object
    /// </summary>
    /// <param name="total">Total grams</param>
    /// <param name="ofWhichSugar">Sugar grams</param>
    /// <returns>Result indicating success, the error that occured and the newly created object</returns>
    public static Result<Carbohydrates> Create(float total, float ofWhichSugar)
    {
        if (total < ofWhichSugar)
        {
            return Result.Failure<Carbohydrates>(
                NutritionalValuesErrors.GramsOfSugarShouldNotExceedGramsOfCarbohydrates);
        }

        var carbohydrates = new Carbohydrates
        {
            Total = total,
            OfWhichSugar = ofWhichSugar
        };
        return Result.Success(carbohydrates);
    }
}
