using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateDishCategory;

public sealed record UpdateDishCategoryCommand(Guid DishId, DishCategory Category) : ICommand;
