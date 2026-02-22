using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

public sealed record NutritionalValues
{
    private NutritionalValues()
    { } 

    public Energy Energy { get; init; }
    public float GramsOfFat { get; init; }
    public Carbohydrates GramsOfCarbohydrates { get; init; }
    public float GramsOfProtein { get; init; }
    public float GramsOfSalt { get; init; }
    
    public static Result<NutritionalValues> Create(
        float kcal,
        float gramsOfFat,
        float gramsOfCarbohydrates,
        float gramsOfSugar,
        float gramsOfProtein,
        float gramsOfSalt)
    {
        Result<Energy> energyResult = Energy.Create(kcal);
        if (energyResult.IsFailure)
        {
            return Result.Failure<NutritionalValues>(energyResult.Error);
        }
        Energy energy = energyResult.Value;
        
        
        Result<Carbohydrates> carbohydratesResult =
            Carbohydrates.Create(gramsOfCarbohydrates, gramsOfSugar);
        if (carbohydratesResult.IsFailure)
        {
            return Result.Failure<NutritionalValues>(carbohydratesResult.Error);
        }
        Carbohydrates carbohydrates = carbohydratesResult.Value;

        return new NutritionalValues
        {
            Energy = energy,
            GramsOfFat = gramsOfFat,
            GramsOfCarbohydrates = carbohydrates,
            GramsOfProtein = gramsOfProtein,
            GramsOfSalt = gramsOfSalt
        };
    }
}
