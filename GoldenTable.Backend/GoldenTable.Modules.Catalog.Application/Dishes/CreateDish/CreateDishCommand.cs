using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;

public sealed record CreateDishCommand(
    string Name,
    string Description,
    decimal BasePriceAmount,
    string BasePriceCurrency,
    List<DishSize> Sizes,
    float Kcal,
    float GramsOfFat,
    float GramsOfCarbohydrates,
    float GramsOfSugar,
    float GramsOfProtein,
    float GramsOfSalt,
    string DishCategory,
    List<DishTag> Tags) : ICommand<Guid>;
