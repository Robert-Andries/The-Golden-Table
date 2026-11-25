using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Exceptions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Dishes.Create;

internal sealed class DishCreatedIntegrationEventHandler(
    IDishRepository dishRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    ILogger<DishCreatedIntegrationEventHandler> logger)
    : IntegrationEventHandler<DishCreatedIntegrationEvent>
{
    public override async Task Handle(
        DishCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        // Delivery is at-least-once: if the copy was saved but the inbox wasn't marked, don't fail on the retry
        if (await dishRepository.GetAsync(integrationEvent.DishId, cancellationToken) is not null)
        {
            DishLogs.DishAlreadyExists(logger, integrationEvent.DishId);
            return;
        }

        Result<Money> basePriceResult =
            Money.Create(integrationEvent.BasePriceAmount, integrationEvent.BasePriceCurrency);
        if (basePriceResult.IsFailure)
        {
            throw Fail(integrationEvent.DishId, basePriceResult.Error);
        }

        var sizes = integrationEvent.Sizes
            .Select(s => new DishSize(s.Name, s.PriceAdded, s.Weight))
            .ToList();

        Result<Dish> dishResult = Dish.Create(
            integrationEvent.DishId,
            new Name(integrationEvent.Name),
            basePriceResult.Value,
            dateTimeProvider.UtcNow,
            sizes);
        if (dishResult.IsFailure)
        {
            throw Fail(integrationEvent.DishId, dishResult.Error);
        }

        await dishRepository.AddAsync(dishResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        DishLogs.DishCopyCreated(logger, integrationEvent.DishId);
    }

    // Throwing lets the inbox job record the error on the message instead of silently dropping the dish
    private GoldenTableException Fail(Guid dishId, Error error)
    {
        DishLogs.CreateCopyError(logger, dishId, error);
        return new GoldenTableException(nameof(DishCreatedIntegrationEventHandler), error);
    }
}
