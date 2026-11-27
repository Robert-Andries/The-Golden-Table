namespace GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

/// <summary>
///     Value object holding the data necessary for a dish size
/// </summary>
/// <param name="Name">The name of the dish size. E.g. small</param>
/// <param name="PriceAdded">The price difference that needs to be added in the BasePrice for that particular size</param>
/// <param name="Weight">Total grams of the dish</param>
public sealed record DishSize(string Name, float PriceAdded, float Weight);
