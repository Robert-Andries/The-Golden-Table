using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Orders.Application.Orders.Create;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Orders;

public sealed class CreateOrder(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_CreateOrder_AndPersistIt()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Dish dish = await PutDishInDbAsync(12.5m);
        Address address = NewAddress();

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(customerId, [(dish.Id, null, 2)], address));

        // Assert
        result.IsSuccess.Should().BeTrue();
        Order dbOrder = await GetOrderFromDbAsync(result.Value.Id);
        dbOrder.CustomerId.Should().Be(customerId);
        dbOrder.Status.Should().Be(OrderStatus.Placed);
        dbOrder.CancelRequested.Should().BeFalse();
        dbOrder.Address.Should().Be(address);
        dbOrder.Items.Should().BeEquivalentTo([new DishItem(dish.Id, null, 2)]);
        dbOrder.TotalPrice.Should().Be(Money.Create(25m, "EUR").Value);
        dbOrder.CreatedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task Should_AddSizePriceToEachUnit_WhenSizeIsSelected()
    {
        // Arrange
        Dish pizza = await PutDishInDbAsync(10m, "EUR", new DishSize("Small", 0, 300), new DishSize("Large", 4, 500));
        Dish drink = await PutDishInDbAsync(3m);

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(
            Guid.NewGuid(),
            [(pizza.Id, "Large", 2), (pizza.Id, "Small", 1), (drink.Id, null, 3)],
            NewAddress()));

        // Assert
        // (10 + 4) * 2 + (10 + 0) * 1 + 3 * 3 = 47
        result.IsSuccess.Should().BeTrue();
        Order dbOrder = await GetOrderFromDbAsync(result.Value.Id);
        dbOrder.TotalPrice.Amount.Should().Be(47m);
        dbOrder.Items.Should().HaveCount(3);
    }

    [Fact]
    public async Task Should_CreateSeparateOrders_WhenCustomerOrdersTwice()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Dish dish = await PutDishInDbAsync(5m);

        // Act
        Result<Order> first = await SendAsync(new CreateOrderCommand(customerId, [(dish.Id, null, 1)], NewAddress()));
        Result<Order> second = await SendAsync(new CreateOrderCommand(customerId, [(dish.Id, null, 1)], NewAddress()));

        // Assert
        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeTrue();
        second.Value.Id.Should().NotBe(first.Value.Id);
        (await Context.Orders.AsNoTracking().CountAsync(o => o.CustomerId == customerId)).Should().Be(2);
    }

    [Fact]
    public async Task Should_WriteOrderCreatedEventToOutbox()
    {
        // Arrange
        Dish dish = await PutDishInDbAsync();

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, null, 1)], NewAddress()));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await WaitForProcessedOutboxMessageAsync("OrderCreatedDomainEvent", result.Value.Id)).Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnError_WhenThereAreNoItems()
    {
        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [], NewAddress()));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidItems);
        await ShouldHaveNoOrdersAsync();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Should_ReturnError_WhenQuantityIsNotPositive(int quantity)
    {
        // Arrange
        Dish dish = await PutDishInDbAsync();

        // Act
        Result<Order> result =
            await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, null, quantity)], NewAddress()));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidQuantity);
        await ShouldHaveNoOrdersAsync();
    }

    [Fact]
    public async Task Should_ReturnError_WhenDishDoesNotExist()
    {
        // Arrange
        Dish dish = await PutDishInDbAsync();

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(
            Guid.NewGuid(),
            [(dish.Id, null, 1), (Guid.NewGuid(), null, 1)],
            NewAddress()));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
        await ShouldHaveNoOrdersAsync();
    }

    [Fact]
    public async Task Should_ReturnError_WhenSizeDoesNotExist()
    {
        // Arrange
        Dish dish = await PutDishInDbAsync(10m, "EUR", new DishSize("Small", 0, 300));

        // Act
        Result<Order> result =
            await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, "Huge", 1)], NewAddress()));

        // Assert
        result.Error.Should().Be(DishErrors.InvalidSize);
        await ShouldHaveNoOrdersAsync();
    }

    [Fact]
    public async Task Should_ReturnError_WhenDishesHaveDifferentCurrencies()
    {
        // Arrange
        Dish inEuro = await PutDishInDbAsync(10m);
        Dish inDollars = await PutDishInDbAsync(10m, "USD");

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(
            Guid.NewGuid(),
            [(inEuro.Id, null, 1), (inDollars.Id, null, 1)],
            NewAddress()));

        // Assert
        result.Error.Should().Be(MoneyErrors.CurrencyDiffer);
        await ShouldHaveNoOrdersAsync();
    }

    [Fact]
    public async Task Should_ReturnError_WhenSizeMakesThePriceNegative()
    {
        // Arrange
        Dish dish = await PutDishInDbAsync(5m, "EUR", new DishSize("Kids", -8, 100));

        // Act
        Result<Order> result =
            await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, "Kids", 1)], NewAddress()));

        // Assert
        result.Error.Should().Be(MoneyErrors.InvalidAmount);
        await ShouldHaveNoOrdersAsync();
    }

    [Fact]
    public async Task Should_ReturnError_WhenAddressIsMissing()
    {
        // Arrange
        Dish dish = await PutDishInDbAsync();

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, null, 1)], null!));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidAddress);
        await ShouldHaveNoOrdersAsync();
    }

    [Theory]
    [InlineData("", "City", "Street", "1", "100000")]
    [InlineData("County", " ", "Street", "1", "100000")]
    [InlineData("County", "City", "", "1", "100000")]
    [InlineData("County", "City", "Street", "", "100000")]
    [InlineData("County", "City", "Street", "1", "")]
    public async Task Should_ReturnError_WhenAddressHasBlankFields(
        string county,
        string city,
        string street,
        string streetNumber,
        string zipCode)
    {
        // Arrange
        Dish dish = await PutDishInDbAsync();
        var address = new Address(county, city, street, streetNumber, zipCode);

        // Act
        Result<Order> result = await SendAsync(new CreateOrderCommand(Guid.NewGuid(), [(dish.Id, null, 1)], address));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidAddress);
        await ShouldHaveNoOrdersAsync();
    }

    private async Task ShouldHaveNoOrdersAsync()
    {
        (await Context.Orders.AsNoTracking().CountAsync()).Should().Be(0);
    }
}
