using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Dishes;

/// <summary>
///     Publishes Catalog integration events on the bus and waits for the Orders inbox to process them,
///     the same path a real event takes: consumer, inbox table, inbox job, handler
/// </summary>
public abstract class DishSyncBaseTest(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    protected static DishCreatedIntegrationEvent DishCreated(
        Guid dishId,
        string name = "Margherita",
        decimal price = 10m,
        string currency = "EUR",
        List<DishSizeModel>? sizes = null)
    {
        // A List, the type the bus hands to consumers: the inbox serializer can't recreate a collection expression
        return new DishCreatedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dishId, name, price, currency,
            sizes ?? []);
    }

    protected async Task<InboxMessage> PublishAndWaitAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent)
        where TIntegrationEvent : Common.Application.EventBus.IIntegrationEvent
    {
        await PublishAsync(integrationEvent);
        return await WaitForProcessedInboxMessageAsync(integrationEvent.Id);
    }

    /// <summary>
    ///     Creates the local copy of a dish through the same event Catalog sends
    /// </summary>
    protected async Task<Dish> PutDishCopyAsync(decimal price = 10m, List<DishSizeModel>? sizes = null)
    {
        var dishId = Guid.NewGuid();
        await PublishAndWaitAsync(DishCreated(dishId, price: price, sizes: sizes));
        return (await FindDishInDbAsync(dishId))!;
    }
}
