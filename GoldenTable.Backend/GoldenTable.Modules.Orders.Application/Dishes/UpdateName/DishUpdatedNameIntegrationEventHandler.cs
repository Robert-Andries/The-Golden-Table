using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.EventBus;
using GoldenTable.Common.Application.Exceptions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Dishes.UpdateName;

/// <summary>
///     Renames the local copy of a dish renamed in the Catalog module
/// </summary>
internal sealed class DishUpdatedNameIntegrationEventHandler(
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    ILogger<DishUpdatedNameIntegrationEventHandler> logger)
    : IntegrationEventHandler<DishUpdatedNameIntegrationEvent>
{
    public override async Task Handle(
        DishUpdatedNameIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        Dish? dish = await dishRepository.GetAsync(integrationEvent.DishId, cancellationToken);
        if (dish is null)
        {
            DishLogs.DishNotFound(logger, integrationEvent.DishId);
            throw new GoldenTableException(nameof(DishUpdatedNameIntegrationEventHandler), DishErrors.NotFound);
        }

        Result result = dish.UpdateName(new Name(integrationEvent.NewName), dateTimeProvider.UtcNow);

        // SameName means the copy is already up to date (e.g. a retried event)
        if (result.IsFailure && result.Error != DishErrors.SameName)
        {
            DishLogs.UpdateCopyError(logger, integrationEvent.DishId, result.Error);
            throw new GoldenTableException(nameof(DishUpdatedNameIntegrationEventHandler), result.Error);
        }

        await dishRepository.UpdateAsync(dish, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await dishCacheService.UpdateAsync(dish, cancellationToken);

        DishLogs.DishCopyUpdated(logger, integrationEvent.DishId);
    }
}
