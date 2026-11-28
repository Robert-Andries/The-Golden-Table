using GoldenTable.Common.Application.EventBus;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;

namespace GoldenTable.Modules.Catalog.IntegrationEvents;

public sealed class DishCreatedIntegrationEvent(
    Guid id,
    DateTime occurredOnUtc,
    Guid dishId,
    string name,
    decimal basePriceAmount,
    string basePriceCurrency,
    IReadOnlyCollection<DishSizeModel> sizes)
    : IntegrationEvent(id, occurredOnUtc)
{
    public Guid DishId { get; init; } = dishId;
    public string Name { get; init; } = name;
    public decimal BasePriceAmount { get; init; } = basePriceAmount;
    public string BasePriceCurrency { get; init; } = basePriceCurrency;
    public IReadOnlyCollection<DishSizeModel> Sizes { get; init; } = sizes;
}
