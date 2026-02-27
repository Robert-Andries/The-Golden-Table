using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateBasePrice;

public sealed record UpdateBasePriceCommand(Guid DishId, decimal NewBasePrice) : ICommand;
