using FluentAssertions;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Dishes;

public sealed class DishCreated(IntegrationTestWebAppFactory factory) : DishSyncBaseTest(factory)
{
    [Fact]
    public async Task Should_CreateDishCopy()
    {
        // Arrange
        var dishId = Guid.NewGuid();
        DishCreatedIntegrationEvent integrationEvent = DishCreated(
            dishId,
            "Quattro Formaggi",
            14.75m,
            "EUR",
            [new DishSizeModel("Small", 0, 300), new DishSizeModel("Large", 4.5m, 550)]);

        // Act
        InboxMessage message = await PublishAndWaitAsync(integrationEvent);

        // Assert
        message.Error.Should().BeNull();
        Dish? dish = await FindDishInDbAsync(dishId);
        dish.Should().NotBeNull();
        dish.Name.Value.Should().Be("Quattro Formaggi");
        dish.BasePrice.Should().Be(Money.Create(14.75m, "EUR").Value);
        dish.Sizes.Should().BeEquivalentTo([new DishSize("Small", 0, 300), new DishSize("Large", 4.5m, 550)]);
    }

    [Fact]
    public async Task Should_CreateDishCopy_WithoutSizes()
    {
        // Arrange
        var dishId = Guid.NewGuid();

        // Act
        InboxMessage message = await PublishAndWaitAsync(DishCreated(dishId));

        // Assert
        message.Error.Should().BeNull();
        (await FindDishInDbAsync(dishId))!.Sizes.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_KeepOneCopy_WhenTheDishIsCreatedTwice()
    {
        // Arrange
        // Two different events for the same dish, e.g. a redelivery that got a new message id
        var dishId = Guid.NewGuid();
        await PublishAndWaitAsync(DishCreated(dishId, "First"));

        // Act
        InboxMessage message = await PublishAndWaitAsync(DishCreated(dishId, "Second"));

        // Assert
        message.Error.Should().BeNull();
        List<Dish> copies = await Context.Dishes.AsNoTracking().Where(d => d.Id == dishId).ToListAsync();
        copies.Should().ContainSingle().Which.Name.Value.Should().Be("First");
    }

    [Fact]
    public async Task Should_StoreTheEventOnce_WhenItIsDeliveredTwice()
    {
        // Arrange
        var dishId = Guid.NewGuid();
        DishCreatedIntegrationEvent integrationEvent = DishCreated(dishId);
        await PublishAndWaitAsync(integrationEvent);

        // Act
        await PublishAsync(integrationEvent);
        // A marker published afterwards on the same queue: once it is processed, the duplicate was handled too
        InboxMessage marker = await PublishAndWaitAsync(DishCreated(Guid.NewGuid()));

        // Assert
        marker.Error.Should().BeNull();
        (await Context.Set<InboxMessage>().AsNoTracking().CountAsync(m => m.Id == integrationEvent.Id))
            .Should().Be(1);
        (await Context.Dishes.AsNoTracking().CountAsync(d => d.Id == dishId)).Should().Be(1);
    }

    [Fact]
    public async Task Should_RecordError_AndCreateNoCopy_WhenCurrencyIsInvalid()
    {
        // Arrange
        var dishId = Guid.NewGuid();

        // Act
        InboxMessage message = await PublishAndWaitAsync(DishCreated(dishId, currency: "XYZ"));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
        (await FindDishInDbAsync(dishId)).Should().BeNull();
    }

    [Fact]
    public async Task Should_RecordError_AndCreateNoCopy_WhenPriceIsNegative()
    {
        // Arrange
        var dishId = Guid.NewGuid();

        // Act
        InboxMessage message = await PublishAndWaitAsync(DishCreated(dishId, price: -1m));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
        (await FindDishInDbAsync(dishId)).Should().BeNull();
    }

    [Fact]
    public async Task Should_RecordError_AndCreateNoCopy_WhenNameIsBlank()
    {
        // Arrange
        var dishId = Guid.NewGuid();

        // Act
        InboxMessage message = await PublishAndWaitAsync(DishCreated(dishId, " "));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
        (await FindDishInDbAsync(dishId)).Should().BeNull();
    }
}
