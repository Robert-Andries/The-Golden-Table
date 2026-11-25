using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Orders.RequestCancel;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Orders;

public sealed class RequestCancel(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_MarkCancelRequested()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);

        // Act
        Result result = await SendAsync(new RequestCancelCommand(customerId, order.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        Order dbOrder = await GetOrderFromDbAsync(order.Id);
        dbOrder.CancelRequested.Should().BeTrue();
        // Requesting is not cancelling: staff decide through the status
        dbOrder.Status.Should().Be(order.Status);
    }

    [Fact]
    public async Task Should_WriteCancelRequestedEventToOutbox()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);

        // Act
        Result result = await SendAsync(new RequestCancelCommand(customerId, order.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await WaitForProcessedOutboxMessageAsync("OrderCancelRequestedDomainEvent", order.Id)).Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnError_WhenCancelWasAlreadyRequested()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        // The first request caches the order, so the second one is checked against the cached copy
        Result first = await SendAsync(new RequestCancelCommand(customerId, order.Id));

        // Act
        Result second = await SendAsync(new RequestCancelCommand(customerId, order.Id));

        // Assert
        first.IsSuccess.Should().BeTrue();
        second.Error.Should().Be(OrderErrors.CancelAlreadyRequested);
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenOrderDoesNotExist()
    {
        // Act
        Result result = await SendAsync(new RequestCancelCommand(Guid.NewGuid(), Guid.NewGuid()));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Should_ReturnNotFound_AndNotCancel_WhenAnotherCustomerPlacedTheOrder()
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        Result result = await SendAsync(new RequestCancelCommand(Guid.NewGuid(), order.Id));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
        (await GetOrderFromDbAsync(order.Id)).CancelRequested.Should().BeFalse();
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Preparing)]
    public async Task Should_MarkCancelRequested_WhenOrderIsInProgress(OrderStatus status)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId, status);

        // Act
        Result result = await SendAsync(new RequestCancelCommand(customerId, order.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await GetOrderFromDbAsync(order.Id)).CancelRequested.Should().BeTrue();
    }

    [Theory]
    [InlineData(OrderStatus.Sent)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Rejected)]
    public async Task Should_ReturnError_AndNotMarkCancel_WhenOrderIsFinished(OrderStatus status)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId, status);

        // Act
        Result result = await SendAsync(new RequestCancelCommand(customerId, order.Id));

        // Assert
        result.Error.Should().Be(OrderErrors.CannotRequestCancel);
        (await GetOrderFromDbAsync(order.Id)).CancelRequested.Should().BeFalse();
    }
}
