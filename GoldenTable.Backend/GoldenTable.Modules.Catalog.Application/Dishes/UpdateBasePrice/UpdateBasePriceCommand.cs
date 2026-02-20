using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateBasePrice;

public sealed record UpdateBasePriceCommand(Guid DishId, decimal NewBasePrice) : ICommand;
