using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.DeleteDish;

public sealed record DeleteDishCommand(Guid DishId) : ICommand;
