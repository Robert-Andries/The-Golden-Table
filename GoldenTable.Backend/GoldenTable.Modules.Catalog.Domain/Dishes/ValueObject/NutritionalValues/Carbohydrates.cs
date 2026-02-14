using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

public sealed record Carbohydrates
{
    private Carbohydrates()
    { }
    public float Total { get; init; }
    public float OfWhichSugar { get; init; }

    public static Result<Carbohydrates> Create(float total, float ofWhichSugar)
    {
        if (total < ofWhichSugar)
        {
            return Result.Failure<Carbohydrates>(NutritionalValuesErrors.GramsOfSugarShouldNotExceedGramsOfCarbohydrates);
        }
        var carbohydrates = new Carbohydrates
        {
            Total = total,
            OfWhichSugar = ofWhichSugar
        };
        return Result.Success(carbohydrates);
    }
}
