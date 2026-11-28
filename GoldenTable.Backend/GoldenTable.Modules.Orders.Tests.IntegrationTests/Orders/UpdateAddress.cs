using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Orders.UpdateAddress;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Orders;

public sealed class UpdateAddress(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Theory]
    [InlineData(OrderStatus.Placed)]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Preparing)]
    public async Task Should_UpdateAddress_WhenOrderIsNotFinished(OrderStatus status)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId, status);
        Address newAddress = NewAddress();

        // Act
        Result result = await SendAsync(new UpdateAddressCommand(customerId, order.Id, newAddress));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await GetOrderFromDbAsync(order.Id)).Address.Should().Be(newAddress);
    }

    [Fact]
    public async Task Should_UpdateAddress_WhenOrderIsCached()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        Address lastAddress = NewAddress();
        // The first change caches the order, so the second one starts from the cached copy
        Result first = await SendAsync(new UpdateAddressCommand(customerId, order.Id, NewAddress()));

        // Act
        Result second = await SendAsync(new UpdateAddressCommand(customerId, order.Id, lastAddress));

        // Assert
        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeTrue();
        (await GetOrderFromDbAsync(order.Id)).Address.Should().Be(lastAddress);
    }

    [Fact]
    public async Task Should_WriteAddressChangedEventToOutbox()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);

        // Act
        Result result = await SendAsync(new UpdateAddressCommand(customerId, order.Id, NewAddress()));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await WaitForProcessedOutboxMessageAsync("OrderAddressChangedDomainEvent", order.Id)).Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenOrderDoesNotExist()
    {
        // Act
        Result result = await SendAsync(new UpdateAddressCommand(Guid.NewGuid(), Guid.NewGuid(), NewAddress()));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Should_ReturnNotFound_AndKeepAddress_WhenAnotherCustomerPlacedTheOrder()
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        Result result = await SendAsync(new UpdateAddressCommand(Guid.NewGuid(), order.Id, NewAddress()));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
        (await GetOrderFromDbAsync(order.Id)).Address.Should().Be(order.Address);
    }

    [Theory]
    [InlineData(OrderStatus.Sent)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Rejected)]
    public async Task Should_ReturnError_WhenOrderIsFinished(OrderStatus status)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId, status);

        // Act
        Result result = await SendAsync(new UpdateAddressCommand(customerId, order.Id, NewAddress()));

        // Assert
        result.Error.Should().Be(OrderErrors.CannotUpdateAddress);
        (await GetOrderFromDbAsync(order.Id)).Address.Should().Be(order.Address);
    }

    [Fact]
    public async Task Should_ReturnError_WhenAddressIsTheSame()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);

        // Act
        Result result = await SendAsync(new UpdateAddressCommand(customerId, order.Id, order.Address with { }));

        // Assert
        result.Error.Should().Be(OrderErrors.SameAddress);
    }

    [Fact]
    public async Task Should_ReturnError_WhenAddressIsMissing()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);

        // Act
        Result result = await SendAsync(new UpdateAddressCommand(customerId, order.Id, null!));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidAddress);
        (await GetOrderFromDbAsync(order.Id)).Address.Should().Be(order.Address);
    }

    [Fact]
    public async Task Should_ReturnError_WhenAddressHasBlankFields()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        Address blankStreet = NewAddress() with { Street = " " };

        // Act
        Result result = await SendAsync(new UpdateAddressCommand(customerId, order.Id, blankStreet));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidAddress);
        (await GetOrderFromDbAsync(order.Id)).Address.Should().Be(order.Address);
    }
}
