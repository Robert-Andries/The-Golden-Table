namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

public sealed record NutritionalValues(Energy Energy, float GramsOfFat, Carbohydrates GramsOfCarbohydrates,
    float GramsOfProtein, float GramsOfSalt);
