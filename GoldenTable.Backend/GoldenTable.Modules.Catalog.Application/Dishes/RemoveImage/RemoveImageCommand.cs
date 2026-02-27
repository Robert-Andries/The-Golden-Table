using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.RemoveImage;

public sealed record RemoveImageCommand(Guid DishId, Guid ImageId) : ICommand;
