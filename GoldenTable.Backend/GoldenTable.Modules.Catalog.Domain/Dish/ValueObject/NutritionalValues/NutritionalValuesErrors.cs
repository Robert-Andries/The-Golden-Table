using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dish.ValueObject.NutritionalValues;

public static class NutritionalValuesErrors
{
    public static Error GramsOfSugarShouldNotExceedGramsOfCarbohydrates { get; }
        = new("NutritionalValuesErrors.GramsOfSugarShouldNotExceedGramsOfCarbohydrates", 
            "The provided grams of Sugar is greater total grams of carbohydrates.", ErrorType.Validation);
}
