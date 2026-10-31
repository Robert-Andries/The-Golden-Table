using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.IntegrationEvents;

namespace GoldenTable.Modules.Catalog.Application.Dishes.EditDish.DomainEvents;

internal sealed class DishUpdatedPriceDomainEventHandler(IEventBus bus) : DomainEventHandler<DishUpdatedPriceDomainEvent>
{
    public override async Task Handle(DishUpdatedPriceDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await bus.PublishAsync(
            new DishUpdatedPriceIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.DishId,
                domainEvent.Amount,
                domainEvent.CurrencyCode),
            cancellationToken);
    }
}
