using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Dishes.Events;

public sealed class DishUpdatedPriceDomainEvent(Guid dishId, decimal amount, string currencyCode)
    : DomainEvent
{
    public Guid DishId { get; init; } = dishId;
    public decimal Amount { get; init; } = amount;
    public string CurrencyCode { get; init; } = currencyCode;
}
