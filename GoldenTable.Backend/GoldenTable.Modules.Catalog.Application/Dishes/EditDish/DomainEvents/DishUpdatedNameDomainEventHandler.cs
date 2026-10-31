using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.IntegrationEvents;

namespace GoldenTable.Modules.Catalog.Application.Dishes.EditDish.DomainEvents;

internal sealed class DishUpdatedNameDomainEventHandler(IEventBus bus) : DomainEventHandler<DishUpdatedNameDomainEvent>
{
    public override async Task Handle(DishUpdatedNameDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await bus.PublishAsync(
            new DishUpdatedNameIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.DishId,
                domainEvent.NewName.Value),
            cancellationToken);
    }
}
