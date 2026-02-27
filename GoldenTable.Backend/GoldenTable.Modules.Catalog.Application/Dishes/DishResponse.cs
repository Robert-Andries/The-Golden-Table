using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Application.Dishes;

public sealed class DishResponse
{
    public DishResponse(Dish dish)
    {
        Name = dish.Name.Value;
        Description = dish.Description.Value;
        BasePriceAmount = dish.BasePrice.Amount;
        BasePriceCurrency = dish.BasePrice.Currency.Code;
        Category = dish.Category.Name;
        Tags = dish.Tags.Select(t => t.Value).ToList();
        NutritionalInformation = dish.NutritionalInformation;
        ImagesUris = dish.Images.Select(i => i.Uri.AbsoluteUri).ToList();
        Sizes = dish.Sizes.ToList();
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public decimal BasePriceAmount { get; set; }
    public string BasePriceCurrency { get; set; }
    public string Category { get; set; }
    public List<string> Tags { get; set; }
    public NutritionalValues NutritionalInformation { get; set; }
    public List<string> ImagesUris { get; set; }
    public List<DishSize> Sizes { get; set; }
}
