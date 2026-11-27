using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Orders.Application.Orders.Create;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Dishes;

public sealed class DishDeleted(IntegrationTestWebAppFactory factory) : DishSyncBaseTest(factory)
{
    [Fact]
    public async Task Should_DeleteDishCopy()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync();

        // Act
        InboxMessage message =
            await PublishAndWaitAsync(new DishDeletedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id));

        // Assert
        message.Error.Should().BeNull();
        (await FindDishInDbAsync(dish.Id)).Should().BeNull();
    }

    [Fact]
    public async Task Should_Succeed_WhenDishCopyIsAlreadyGone()
    {
        // Act
        InboxMessage message =
            await PublishAndWaitAsync(new DishDeletedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid()));

        // Assert
        message.Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_RejectNewOrders_WhenTheDishWasCachedBeforeDeletion()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync();
        // Renaming caches the dish and ordering reads the cache first, so deleting must drop the cache entry too
        await PublishAndWaitAsync(
            new DishUpdatedNameIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, "Soon gone"));
        await PublishAndWaitAsync(new DishDeletedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id));

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, null, 1)], NewAddress()));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }
}
