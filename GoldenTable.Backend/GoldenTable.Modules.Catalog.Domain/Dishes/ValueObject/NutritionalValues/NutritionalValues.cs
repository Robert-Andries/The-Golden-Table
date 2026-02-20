using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

public sealed record NutritionalValues(
    Energy Energy,
    float GramsOfFat,
    Carbohydrates GramsOfCarbohydrates,
    float GramsOfProtein,
    float GramsOfSalt)
{
    public static Result<NutritionalValues> Create(
        float kcal,
        float gramsOfFat,
        float gramsOfCarbohydrates,
        float gramsOfSugar,
        float gramsOfProtein,
        float gramsOfSalt)
    {
        Energy energy = new(kcal);
        
        Result<Carbohydrates> carbohydratesResult =
            Carbohydrates.Create(gramsOfCarbohydrates, gramsOfSugar);
        if (carbohydratesResult.IsFailure)
        {
            return Result.Failure<NutritionalValues>(carbohydratesResult.Error);
        }
        Carbohydrates carbohydrates = carbohydratesResult.Value;
        
        return new NutritionalValues(
            energy,
            gramsOfFat,
            carbohydrates,
            gramsOfProtein,
            gramsOfSalt);
    }
}
