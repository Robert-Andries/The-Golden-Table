using GoldenTable.Common.Application.EventBus;

namespace GoldenTable.Modules.Catalog.IntegrationEvents;

public sealed class DishUpdatedNameIntegrationEvent(Guid id, DateTime occurredOnUtc, Guid dishId, string newName)
    : IntegrationEvent(id, occurredOnUtc)
{
    public Guid DishId { get; } = dishId;
    public string NewName { get; } = newName;
}
