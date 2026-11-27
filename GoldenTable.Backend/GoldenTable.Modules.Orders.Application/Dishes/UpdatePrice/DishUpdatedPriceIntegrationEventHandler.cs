using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Exceptions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Dishes.UpdatePrice;

/// <summary>
///     Updates the base price of the local copy of a dish repriced in the Catalog module
/// </summary>
internal sealed class DishUpdatedPriceIntegrationEventHandler(
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    ILogger<DishUpdatedPriceIntegrationEventHandler> logger)
    : IntegrationEventHandler<DishUpdatedPriceIntegrationEvent>
{
    public override async Task Handle(
        DishUpdatedPriceIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        Dish? dish = await dishRepository.GetAsync(integrationEvent.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, integrationEvent.DishId);
            throw new GoldenTableException(nameof(DishUpdatedPriceIntegrationEventHandler), DishErrors.NotFound);
        }

        Result<Money> priceResult =
            Money.Create(integrationEvent.BasePriceAmount, integrationEvent.BasePriceCurrency);
        if (priceResult.IsFailure)
        {
            DishLogs.UpdateCopyError(logger, integrationEvent.DishId, priceResult.Error);
            throw new GoldenTableException(nameof(DishUpdatedPriceIntegrationEventHandler), priceResult.Error);
        }

        Result result = dish.UpdateBasePrice(priceResult.Value, dateTimeProvider.UtcNow);

        // SamePrice means the copy is already up to date (e.g. a retried event)
        if (result.IsFailure && result.Error != DishErrors.SamePrice)
        {
            DishLogs.UpdateCopyError(logger, integrationEvent.DishId, result.Error);
            throw new GoldenTableException(nameof(DishUpdatedPriceIntegrationEventHandler), result.Error);
        }

        await dishRepository.UpdateAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);

        DishLogs.DishCopyUpdated(logger, integrationEvent.DishId);
    }
}
