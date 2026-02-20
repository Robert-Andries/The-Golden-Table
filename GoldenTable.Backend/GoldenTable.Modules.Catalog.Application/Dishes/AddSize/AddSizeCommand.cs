using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Application.Dishes.AddSize;

public sealed record AddSizeCommand(Guid DishId, string SizeName, float SizePriceAdded, float SizeWeight) : ICommand;
