using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Orders.ModifyItems;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Orders;

public sealed class ModifyItems(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_ReplaceItems_AndRecalculateTotal()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId, createdOnUtc: DateTime.UtcNow.AddHours(-1));
        Dish pizza = await PutDishInDbAsync(10m, "EUR", new DishSize("Large", 4, 500));
        Dish drink = await PutDishInDbAsync(3m);

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(
            customerId,
            order.Id,
            [(pizza.Id, "Large", 2), (drink.Id, null, 1)]));

        // Assert
        result.IsSuccess.Should().BeTrue();
        Order dbOrder = await GetOrderFromDbAsync(order.Id);
        dbOrder.Items.Should().BeEquivalentTo([new DishItem(pizza.Id, "Large", 2), new DishItem(drink.Id, null, 1)]);
        dbOrder.TotalPrice.Amount.Should().Be(31m);
        dbOrder.ModifiedOnUtc.Should().BeAfter(order.ModifiedOnUtc);
    }

    [Fact]
    public async Task Should_ReplaceItems_WhenOrderIsCached()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        Dish dish = await PutDishInDbAsync(2m);
        // The first change caches the order, so the second one starts from the cached copy
        Result first = await SendAsync(new ModifyItemsCommand(customerId, order.Id, [(dish.Id, null, 1)]));

        // Act
        Result second = await SendAsync(new ModifyItemsCommand(
            customerId,
            order.Id,
            [(dish.Id, null, 5), (order.Items.First().DishId, null, 1)]));

        // Assert
        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeTrue();
        Order dbOrder = await GetOrderFromDbAsync(order.Id);
        dbOrder.Items.Should().HaveCount(2);
        dbOrder.Items.Should().ContainEquivalentOf(new DishItem(dish.Id, null, 5));
        dbOrder.TotalPrice.Amount.Should().Be(10m + order.TotalPrice.Amount);
    }

    [Fact]
    public async Task Should_WriteItemsChangedEventToOutbox()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        Dish dish = await PutDishInDbAsync();

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(customerId, order.Id, [(dish.Id, null, 1)]));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await WaitForProcessedOutboxMessageAsync("OrderItemsChangedDomainEvent", order.Id)).Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        Dish dish = await PutDishInDbAsync();

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(Guid.NewGuid(), Guid.NewGuid(), [(dish.Id, null, 1)]));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Should_ReturnNotFound_AndKeepItems_WhenAnotherCustomerPlacedTheOrder()
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid());
        Dish dish = await PutDishInDbAsync();

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(Guid.NewGuid(), order.Id, [(dish.Id, null, 3)]));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
        Order dbOrder = await GetOrderFromDbAsync(order.Id);
        dbOrder.Items.Should().BeEquivalentTo(order.Items);
        dbOrder.TotalPrice.Should().Be(order.TotalPrice);
    }

    [Fact]
    public async Task Should_ReturnError_WhenThereAreNoItems()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(customerId, order.Id, []));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidItems);
        (await GetOrderFromDbAsync(order.Id)).Items.Should().BeEquivalentTo(order.Items);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task Should_ReturnError_WhenQuantityIsNotPositive(int quantity)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        Dish dish = await PutDishInDbAsync();

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(customerId, order.Id, [(dish.Id, null, quantity)]));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidQuantity);
        (await GetOrderFromDbAsync(order.Id)).Items.Should().BeEquivalentTo(order.Items);
    }

    [Fact]
    public async Task Should_ReturnError_WhenDishDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(customerId, order.Id, [(Guid.NewGuid(), null, 1)]));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
        (await GetOrderFromDbAsync(order.Id)).Items.Should().BeEquivalentTo(order.Items);
    }

    [Fact]
    public async Task Should_ReturnError_WhenSizeDoesNotExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        Dish dish = await PutDishInDbAsync(10m, "EUR", new DishSize("Small", 0, 300));

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(customerId, order.Id, [(dish.Id, "Huge", 1)]));

        // Assert
        result.Error.Should().Be(DishErrors.InvalidSize);
        (await GetOrderFromDbAsync(order.Id)).Items.Should().BeEquivalentTo(order.Items);
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Preparing)]
    public async Task Should_ReplaceItems_WhenOrderIsInProgress(OrderStatus status)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId, status);
        Dish dish = await PutDishInDbAsync(7m);

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(customerId, order.Id, [(dish.Id, null, 2)]));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await GetOrderFromDbAsync(order.Id)).Items.Should().BeEquivalentTo([new DishItem(dish.Id, null, 2)]);
    }

    [Theory]
    [InlineData(OrderStatus.Sent)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Rejected)]
    public async Task Should_ReturnError_AndKeepItems_WhenOrderIsFinished(OrderStatus status)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId, status);
        Dish dish = await PutDishInDbAsync();

        // Act
        Result result = await SendAsync(new ModifyItemsCommand(customerId, order.Id, [(dish.Id, null, 4)]));

        // Assert
        result.Error.Should().Be(OrderErrors.CannotModifyItems);
        Order dbOrder = await GetOrderFromDbAsync(order.Id);
        dbOrder.Items.Should().BeEquivalentTo(order.Items);
        dbOrder.TotalPrice.Should().Be(order.TotalPrice);
    }
}
