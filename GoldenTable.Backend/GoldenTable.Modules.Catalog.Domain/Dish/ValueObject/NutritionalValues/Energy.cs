namespace GoldenTable.Modules.Catalog.Domain.Dish.ValueObject.NutritionalValues;

public sealed record Energy(float KCal)
{
    private const float KCalToKjValue = (float)4.184;
    
    public float Kj => KCal * KCalToKjValue;
}
