using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Orders.Domain.Dishes.Events;

public sealed class DishUpdatedPriceDomainEvent(Guid dishId, decimal newBasePriceAmount, string newBasePriceCurrency)
    : DomainEvent
{
    public Guid DishId { get; } = dishId;
    public decimal NewBasePriceAmount { get; } = newBasePriceAmount;
    public string NewBasePriceCurrency { get; } = newBasePriceCurrency;
}
