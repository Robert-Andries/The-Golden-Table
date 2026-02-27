using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

/// <summary>
///     Singleton holding all the possible errors that the NutritionalValues can potentionally have
/// </summary>
public static class NutritionalValuesErrors
{
    public static Error GramsOfSugarShouldNotExceedGramsOfCarbohydrates { get; }
        = new("NutritionalValuesErrors.GramsOfSugarShouldNotExceedGramsOfCarbohydrates",
            "The provided grams of Sugar is greater total grams of carbohydrates.", ErrorType.Validation);

    public static Error InvalidKcal { get; } = new("NutritionalValuesErrors.InvalidKcal",
        "The provided kcal cannot be less then 0.", ErrorType.Validation);
}
