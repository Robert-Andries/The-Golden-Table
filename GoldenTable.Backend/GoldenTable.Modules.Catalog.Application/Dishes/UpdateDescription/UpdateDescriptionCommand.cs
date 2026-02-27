using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateDescription;

public sealed record UpdateDescriptionCommand(Guid DishId, string newDescription) : ICommand;
