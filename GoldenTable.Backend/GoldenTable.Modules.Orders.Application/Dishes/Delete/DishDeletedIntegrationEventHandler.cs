using GoldenTable.Common.Application.EventBus;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Orders.Application.Abstractions.Data;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Orders.Application.Dishes.Delete;

/// <summary>
///     Removes the local copy of a dish deleted in the Catalog module
/// </summary>
internal sealed class DishDeletedIntegrationEventHandler(
    IDishRepository dishRepository,
    IDishCacheService dishCacheService,
    IUnitOfWork unitOfWork,
    ILogger<DishDeletedIntegrationEventHandler> logger)
    : IntegrationEventHandler<DishDeletedIntegrationEvent>
{
    public override async Task Handle(
        DishDeletedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        Dish? dish = await dishRepository.GetAsync(integrationEvent.DishId, cancellationToken);

        // Already gone (e.g. a retry after the delete was saved): nothing left to do but drop the cache entry
        if (dish is not null)
        {
            dishRepository.Remove(dish);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            DishLogs.DishNotFound(logger, integrationEvent.DishId);
        }

        await dishCacheService.RemoveAsync(integrationEvent.DishId, cancellationToken);

        DishLogs.DishCopyDeleted(logger, integrationEvent.DishId);
    }
}
