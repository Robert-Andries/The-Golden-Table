using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;

namespace GoldenTable.Modules.Order.Domain.Dishes.Events;

public sealed class DishUpdatedPriceDomainEvent(Guid dishId, decimal newBasePriceAmount, string newBasePriceCurrency) : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public decimal NewBasePriceAmount { get; } = newBasePriceAmount;
    public string NewBasePriceCurrency { get; } = newBasePriceCurrency;
}
