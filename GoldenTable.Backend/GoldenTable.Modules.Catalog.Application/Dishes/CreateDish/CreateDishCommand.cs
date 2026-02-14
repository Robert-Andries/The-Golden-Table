using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Enums;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;

public sealed record CreateDishCommand(string Name, string Description, Money BasePrice,
    List<DishSize> Sizes, NutritionalValues NutritionalInformation, List<Guid> ImageIds,
    DishCategory DishCategory, List<DishTag> Tags) : ICommand<Dish>;
