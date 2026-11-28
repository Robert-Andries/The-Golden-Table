using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Orders.ChangeStatus;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Orders;

public sealed class ChangeStatus(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Theory]
    [InlineData(OrderStatus.Placed, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Placed, OrderStatus.Rejected)]
    [InlineData(OrderStatus.Placed, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Preparing)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Sent)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Cancelled)]
    public async Task Should_ChangeStatus_WhenTransitionIsAllowed(OrderStatus from, OrderStatus to)
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid(), from);

        // Act
        Result result = await SendAsync(new ChangeStatusCommand(order.Id, to));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await GetOrderFromDbAsync(order.Id)).Status.Should().Be(to);
    }

    [Theory]
    [InlineData(OrderStatus.Placed, OrderStatus.Preparing)]
    [InlineData(OrderStatus.Placed, OrderStatus.Sent)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Placed)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Rejected)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Sent)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Sent, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Rejected, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Confirmed)]
    public async Task Should_ReturnError_WhenTransitionIsNotAllowed(OrderStatus from, OrderStatus to)
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid(), from);

        // Act
        Result result = await SendAsync(new ChangeStatusCommand(order.Id, to));

        // Assert
        result.Error.Should().Be(OrderErrors.InvalidStatus);
        (await GetOrderFromDbAsync(order.Id)).Status.Should().Be(from);
    }

    [Fact]
    public async Task Should_Succeed_WhenStatusIsTheSame()
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid(), OrderStatus.Confirmed);

        // Act
        Result result = await SendAsync(new ChangeStatusCommand(order.Id, OrderStatus.Confirmed));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await GetOrderFromDbAsync(order.Id)).Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Should_WalkTheWholeLifecycle_UsingTheCachedOrder()
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        // Every step after the first starts from the order cached by the previous step
        Result confirmed = await SendAsync(new ChangeStatusCommand(order.Id, OrderStatus.Confirmed));
        Result preparing = await SendAsync(new ChangeStatusCommand(order.Id, OrderStatus.Preparing));
        Result sent = await SendAsync(new ChangeStatusCommand(order.Id, OrderStatus.Sent));

        // Assert
        confirmed.IsSuccess.Should().BeTrue();
        preparing.IsSuccess.Should().BeTrue();
        sent.IsSuccess.Should().BeTrue();
        (await GetOrderFromDbAsync(order.Id)).Status.Should().Be(OrderStatus.Sent);
    }

    [Fact]
    public async Task Should_ChangeStatus_OfAnyCustomersOrder()
    {
        // Arrange
        // Staff move every order along, so there is no ownership check
        Order first = await PutOrderInDbAsync(Guid.NewGuid());
        Order second = await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        Result firstResult = await SendAsync(new ChangeStatusCommand(first.Id, OrderStatus.Confirmed));
        Result secondResult = await SendAsync(new ChangeStatusCommand(second.Id, OrderStatus.Rejected));

        // Assert
        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Should_WriteStatusChangedEventToOutbox()
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        Result result = await SendAsync(new ChangeStatusCommand(order.Id, OrderStatus.Confirmed));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await WaitForProcessedOutboxMessageAsync("OrderStatusChangedDomainEvent", order.Id)).Error.Should().BeNull();
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenOrderDoesNotExist()
    {
        // Act
        Result result = await SendAsync(new ChangeStatusCommand(Guid.NewGuid(), OrderStatus.Confirmed));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
    }
}
