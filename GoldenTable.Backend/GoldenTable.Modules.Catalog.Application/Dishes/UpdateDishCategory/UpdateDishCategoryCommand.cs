using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateDishCategory;

public sealed record UpdateDishCategoryCommand(Guid DishId, string Category) : ICommand;
