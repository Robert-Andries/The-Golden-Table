using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Application.Orders.GetOrders;
using GoldenTable.Modules.Orders.Application.Orders.GetOrdersByCustomer;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Orders;

public sealed class GetOrders(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_ReturnEveryCustomersOrders_NewestFirst()
    {
        // Arrange
        Order oldest = await PutOrderInDbAsync(Guid.NewGuid(), createdOnUtc: DateTime.UtcNow.AddHours(-3));
        Order newest = await PutOrderInDbAsync(Guid.NewGuid(), createdOnUtc: DateTime.UtcNow);
        Order middle = await PutOrderInDbAsync(Guid.NewGuid(), createdOnUtc: DateTime.UtcNow.AddHours(-1));

        // Act
        Result<IReadOnlyCollection<Order>> result = await SendAsync(new GetOrdersQuery());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Select(o => o.Id).Should().Equal(newest.Id, middle.Id, oldest.Id);
    }

    [Fact]
    public async Task Should_ReturnEmptyList_WhenThereAreNoOrders()
    {
        // Act
        Result<IReadOnlyCollection<Order>> result = await SendAsync(new GetOrdersQuery());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_ReturnOnlyTheCustomersOrders_NewestFirst()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        Order older = await PutOrderInDbAsync(customerId, createdOnUtc: DateTime.UtcNow.AddHours(-2));
        Order newer = await PutOrderInDbAsync(customerId, createdOnUtc: DateTime.UtcNow);
        await PutOrderInDbAsync(Guid.NewGuid());
        await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        Result<IReadOnlyCollection<Order>> result = await SendAsync(new GetOrdersByCustomerQuery(customerId));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Select(o => o.Id).Should().Equal(newer.Id, older.Id);
        result.Value.Should().OnlyContain(o => o.CustomerId == customerId);
    }

    [Fact]
    public async Task Should_ReturnEmptyList_WhenCustomerHasNoOrders()
    {
        // Arrange
        await PutOrderInDbAsync(Guid.NewGuid());

        // Act
        Result<IReadOnlyCollection<Order>> result = await SendAsync(new GetOrdersByCustomerQuery(Guid.NewGuid()));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
