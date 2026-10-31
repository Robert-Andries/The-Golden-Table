using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.IntegrationEvents;

namespace GoldenTable.Modules.Catalog.Application.Dishes.DeleteDish;

internal sealed class DishDeletedDomainEventHandler(IEventBus bus) : DomainEventHandler<DishDeletedDomainEvent>
{
    public override async Task Handle(DishDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await bus.PublishAsync(
            new DishDeletedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.DishId),
            cancellationToken);
    }
}
