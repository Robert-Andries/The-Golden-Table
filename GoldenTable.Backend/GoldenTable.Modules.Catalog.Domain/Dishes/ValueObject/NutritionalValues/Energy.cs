using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

public sealed record Energy
{
    private Energy() 
    { }
    
    public float Kcal { get; init; }
    public float Kj => Kcal * KCalToKjValue;

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
    
    private const float KCalToKjValue = (float)4.184;
}
