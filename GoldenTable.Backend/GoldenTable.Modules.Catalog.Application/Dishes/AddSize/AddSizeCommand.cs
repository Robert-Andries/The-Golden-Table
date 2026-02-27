using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.AddSize;

public sealed record AddSizeCommand(Guid DishId, string SizeName, float SizePriceAdded, float SizeWeight) : ICommand;
