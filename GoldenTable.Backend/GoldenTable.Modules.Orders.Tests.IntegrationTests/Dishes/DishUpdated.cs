using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Common.Infrastructure.Inbox;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;
using GoldenTable.Modules.Orders.Application.Orders.Create;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Dishes;

public sealed class DishUpdated(IntegrationTestWebAppFactory factory) : DishSyncBaseTest(factory)
{
    [Fact]
    public async Task Should_RenameDishCopy()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync();

        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedNameIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, "Diavola"));

        // Assert
        message.Error.Should().BeNull();
        (await FindDishInDbAsync(dish.Id))!.Name.Value.Should().Be("Diavola");
    }

    [Fact]
    public async Task Should_Succeed_WhenNameIsAlreadyUpToDate()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync();

        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedNameIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, dish.Name.Value));

        // Assert
        message.Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_RecordError_WhenRenamingAMissingDish()
    {
        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedNameIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), "Diavola"));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_RecordError_WhenNewNameIsBlank()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync();

        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedNameIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, ""));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
        (await FindDishInDbAsync(dish.Id))!.Name.Should().Be(dish.Name);
    }

    [Fact]
    public async Task Should_UpdateDishCopyPrice()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(10m);

        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedPriceIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, 12.5m, "EUR"));

        // Assert
        message.Error.Should().BeNull();
        (await FindDishInDbAsync(dish.Id))!.BasePrice.Should().Be(Money.Create(12.5m, "EUR").Value);
    }

    [Fact]
    public async Task Should_PriceNewOrdersWithTheLatestPrice_WhenTheDishIsCached()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(10m);
        // Every update caches the dish and ordering reads the cache first, so the cache must follow each change
        await PublishAndWaitAsync(
            new DishUpdatedPriceIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, 20m, "EUR"));
        await PublishAndWaitAsync(
            new DishUpdatedPriceIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, 30m, "EUR"));

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, null, 2)], NewAddress()));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPrice.Amount.Should().Be(60m);
    }

    [Fact]
    public async Task Should_PriceNewOrdersWithTheLatestSizes_WhenTheDishIsCached()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(10m, [new DishSizeModel("Large", 2, 500)]);
        await PublishAndWaitAsync(new DishUpdatedSizesIntegrationEvent(
            Guid.NewGuid(), DateTime.UtcNow, dish.Id, [new DishSizeModel("Large", 5, 500)]));

        // Act
        Result<Order> result =
            await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, "Large", 1)], NewAddress()));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalPrice.Amount.Should().Be(15m);
    }

    [Fact]
    public async Task Should_Succeed_WhenPriceIsAlreadyUpToDate()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(10m);

        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedPriceIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, 10m, "EUR"));

        // Assert
        message.Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_RecordError_WhenRepricingAMissingDish()
    {
        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedPriceIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), 12m, "EUR"));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(-1, "EUR")]
    [InlineData(12, "XYZ")]
    public async Task Should_RecordError_AndKeepPrice_WhenNewPriceIsInvalid(decimal amount, string currency)
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(10m);

        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedPriceIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, amount, currency));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
        (await FindDishInDbAsync(dish.Id))!.BasePrice.Amount.Should().Be(10m);
    }

    [Fact]
    public async Task Should_ReplaceDishCopySizes()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(sizes: [new DishSizeModel("Small", 0, 300)]);
        List<DishSizeModel> newSizes = [new("Medium", 2, 400), new("Large", 4, 500)];

        // Act
        InboxMessage message = await PublishAndWaitAsync(
            new DishUpdatedSizesIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, dish.Id, newSizes));

        // Assert
        message.Error.Should().BeNull();
        (await FindDishInDbAsync(dish.Id))!.Sizes.Should()
            .BeEquivalentTo([new DishSize("Medium", 2, 400), new DishSize("Large", 4, 500)]);
    }

    [Fact]
    public async Task Should_Succeed_WhenSizesAreAlreadyUpToDate()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(sizes: [new DishSizeModel("Small", 0, 300)]);

        // Act
        InboxMessage message = await PublishAndWaitAsync(new DishUpdatedSizesIntegrationEvent(
            Guid.NewGuid(), DateTime.UtcNow, dish.Id, [new DishSizeModel("Small", 0, 300)]));

        // Assert
        message.Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_RecordError_WhenResizingAMissingDish()
    {
        // Act
        InboxMessage message = await PublishAndWaitAsync(new DishUpdatedSizesIntegrationEvent(
            Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), [new DishSizeModel("Small", 0, 300)]));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_RecordError_AndKeepSizes_WhenSizeNamesRepeat()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(sizes: [new DishSizeModel("Small", 0, 300)]);

        // Act
        InboxMessage message = await PublishAndWaitAsync(new DishUpdatedSizesIntegrationEvent(
            Guid.NewGuid(), DateTime.UtcNow, dish.Id, [new DishSizeModel("Large", 2, 400), new DishSizeModel("Large", 3, 500)]));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
        (await FindDishInDbAsync(dish.Id))!.Sizes.Should().BeEquivalentTo([new DishSize("Small", 0, 300)]);
    }

    [Fact]
    public async Task Should_RecordError_AndKeepSizes_WhenASizeIsInvalid()
    {
        // Arrange
        Dish dish = await PutDishCopyAsync(sizes: [new DishSizeModel("Small", 0, 300)]);

        // Act
        InboxMessage message = await PublishAndWaitAsync(new DishUpdatedSizesIntegrationEvent(
            Guid.NewGuid(), DateTime.UtcNow, dish.Id, [new DishSizeModel("Large", 2, 0)]));

        // Assert
        message.Error.Should().NotBeNullOrEmpty();
        (await FindDishInDbAsync(dish.Id))!.Sizes.Should().BeEquivalentTo([new DishSize("Small", 0, 300)]);
    }
}
