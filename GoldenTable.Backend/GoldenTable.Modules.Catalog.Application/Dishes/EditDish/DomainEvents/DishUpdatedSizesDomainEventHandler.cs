using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;

namespace GoldenTable.Modules.Catalog.Application.Dishes.EditDish.DomainEvents;

internal sealed class DishUpdatedSizesDomainEventHandler(IEventBus bus) : DomainEventHandler<DishUpdatedSizesDomainEvent>
{
    public override async Task Handle(DishUpdatedSizesDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await bus.PublishAsync(
            new DishUpdatedSizesIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.DishId,
                domainEvent.Sizes.Select(s => new DishSizeModel(s.Name, s.PriceAdded, s.Weight)).ToList()),
            cancellationToken);
    }
}
