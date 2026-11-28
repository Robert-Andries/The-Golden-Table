using GoldenTable.Common.Application.EventBus;

namespace GoldenTable.Modules.Catalog.IntegrationEvents;

public sealed class DishUpdatedPriceIntegrationEvent(
    Guid id,
    DateTime occurredOnUtc,
    Guid dishId,
    decimal basePriceAmount,
    string basePriceCurrency)
    : IntegrationEvent(id, occurredOnUtc)
{
    public Guid DishId { get; } = dishId;
    public decimal BasePriceAmount { get; } = basePriceAmount;
    public string BasePriceCurrency { get; } = basePriceCurrency;
}
