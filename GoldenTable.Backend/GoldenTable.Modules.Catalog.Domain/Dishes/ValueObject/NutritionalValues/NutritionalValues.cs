using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

/// <summary>
/// Value object holding the legal data needed to display for a dish
/// </summary>
public sealed record NutritionalValues
{
    private NutritionalValues()
    { } 

    /// <summary>
    /// The amount of energy the dish has
    /// </summary>
    public Energy Energy { get; init; }
    /// <summary>
    /// The grams of fat that the dish has
    /// </summary>
    public float GramsOfFat { get; init; }
    /// <summary>
    /// Grams of carbohydrates that the dish has
    /// </summary>
    public Carbohydrates GramsOfCarbohydrates { get; init; }
    /// <summary>
    /// Grams of protein that the dish has
    /// </summary>
    public float GramsOfProtein { get; init; }
    /// <summary>
    /// Grams of salt that the dish has
    /// </summary>
    public float GramsOfSalt { get; init; }
    
    /// <summary>
    /// Factory method used to create an NutritionalValues object
    /// </summary>
    /// <returns>Result indicating success, the error that occured and the newly created object</returns>
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
