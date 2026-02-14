using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Application.Dishes.RemoveSize;

public sealed record RemoveSizeCommand(Guid DishId, DishSize Size) : ICommand;  
