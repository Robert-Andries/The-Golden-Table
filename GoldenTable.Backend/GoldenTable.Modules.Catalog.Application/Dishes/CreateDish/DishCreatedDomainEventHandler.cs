using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Exceptions;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishById;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;
using MediatR;

namespace GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;

internal sealed class DishCreatedDomainEventHandler(ISender sender, IEventBus bus) : DomainEventHandler<DishCreatedDomainEvent>
{
    public override async Task Handle(DishCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Result<DishResponse> dishResult = await sender.Send(new GetDishByIdQuery(domainEvent.DishId), cancellationToken);

        if (dishResult.IsFailure)
        {
            throw new GoldenTableException(nameof(GetDishByIdQuery), dishResult.Error);
        }

        DishResponse dishResponse = dishResult.Value;

        await bus.PublishAsync(
            new DishCreatedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                dishResponse.Id,
                dishResponse.Name,
                dishResponse.BasePriceAmount,
                dishResponse.BasePriceCurrency,
                dishResponse.Sizes.Select(s => new DishSizeModel(s.Name, s.PriceAdded, s.Weight)).ToList()), 
            cancellationToken);
    }
}
