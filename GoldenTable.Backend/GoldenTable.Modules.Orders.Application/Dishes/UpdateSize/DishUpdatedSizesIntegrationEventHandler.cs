using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Exceptions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Dishes.UpdateSize;

/// <summary>
///     Replaces the sizes of the local copy of a dish whose sizes changed in the Catalog module
/// </summary>
internal sealed class DishUpdatedSizesIntegrationEventHandler(
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    ILogger<DishUpdatedSizesIntegrationEventHandler> logger)
    : IntegrationEventHandler<DishUpdatedSizesIntegrationEvent>
{
    public override async Task Handle(
        DishUpdatedSizesIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        Dish? dish = await dishRepository.GetAsync(integrationEvent.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, integrationEvent.DishId);
            throw new GoldenTableException(nameof(DishUpdatedSizesIntegrationEventHandler), DishErrors.NotFound);
        }

        var sizes = integrationEvent.Sizes
            .Select(s => new DishSize(s.Name, s.PriceAdded, s.Weight))
            .ToList();

        // Same sizes already present returns success, so a retried event is a no-op
        Result result = dish.UpdateSizes(sizes, dateTimeProvider.UtcNow);
        if (result.IsFailure)
        {
            DishLogs.UpdateCopyError(logger, integrationEvent.DishId, result.Error);
            throw new GoldenTableException(nameof(DishUpdatedSizesIntegrationEventHandler), result.Error);
        }

        await dishRepository.UpdateAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);

        DishLogs.DishCopyUpdated(logger, integrationEvent.DishId);
    }
}
