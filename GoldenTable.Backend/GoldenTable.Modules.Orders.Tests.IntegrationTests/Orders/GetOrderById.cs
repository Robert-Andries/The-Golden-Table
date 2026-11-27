using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Orders.GetOrderById;
using GoldenTable.Modules.Orders.Application.Orders.RequestCancel;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Orders;

public sealed class GetOrderById(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_ReturnAnyOrder_WhenNoCustomerIsGiven()
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        Result<Order> result = await SendAsync(new GetOrderByIdQuery(order.Id, null));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(order.Id);
        result.Value.CustomerId.Should().Be(order.CustomerId);
        result.Value.Items.Should().BeEquivalentTo(order.Items);
        result.Value.Address.Should().Be(order.Address);
        result.Value.TotalPrice.Should().Be(order.TotalPrice);
    }

    [Fact]
    public async Task Should_ReturnOrder_WhenCustomerPlacedIt()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);

        // Act
        Result<Order> result = await SendAsync(new GetOrderByIdQuery(order.Id, customerId));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenAnotherCustomerPlacedIt()
    {
        // Arrange
        Order order = await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        Result<Order> result = await SendAsync(new GetOrderByIdQuery(order.Id, Guid.NewGuid()));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenOrderDoesNotExist()
    {
        // Act
        Result<Order> result = await SendAsync(new GetOrderByIdQuery(Guid.NewGuid(), null));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Should_ReturnLatestState_WhenOrderIsCached()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        // Updating an order also stores it in the cache
        Result cancelResult = await SendAsync(new RequestCancelCommand(customerId, order.Id));

        // Act
        Result<Order> result = await SendAsync(new GetOrderByIdQuery(order.Id, customerId));

        // Assert
        cancelResult.IsSuccess.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Value.CancelRequested.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnNotFound_WhenCachedOrderBelongsToAnotherCustomer()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order order = await PutOrderInDbAsync(customerId);
        await SendAsync(new RequestCancelCommand(customerId, order.Id));

        // Act
        Result<Order> result = await SendAsync(new GetOrderByIdQuery(order.Id, Guid.NewGuid()));

        // Assert
        result.Error.Should().Be(OrderErrors.NotFound);
    }
}
