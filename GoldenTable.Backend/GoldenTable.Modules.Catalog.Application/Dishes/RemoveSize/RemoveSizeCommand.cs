using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.RemoveSize;

public sealed record RemoveSizeCommand(Guid DishId, string SizeName) : ICommand;
