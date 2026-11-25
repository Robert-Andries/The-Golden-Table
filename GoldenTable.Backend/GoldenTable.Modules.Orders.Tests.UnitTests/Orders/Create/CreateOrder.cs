using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Common.Tests.UnitTests;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Events;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Tests.UnitTests.Orders.Create;

public sealed class CreateOrder : BaseTest
{
    private readonly Address _validAddress;
    private readonly Guid _validCustomerId;
    private readonly List<DishItem> _validItems;
    private readonly DateTime _validSometimeUtc;
    private readonly Money _validTotalPrice;

    public CreateOrder()
    {
        _validCustomerId = Guid.NewGuid();
        _validTotalPrice = Money.Create(Faker.Random.Decimal(1M, 500M), "RON").Value;
        _validItems =
        [
            new DishItem(Guid.NewGuid(), "Small", 2),
            new DishItem(Guid.NewGuid(), "Large", 1)
        ];
        _validAddress = new Address("Cluj", "Cluj-Napoca", "Strada Memorandumului", "28", "400114");
        _validSometimeUtc = DateTime.UtcNow;
    }

    [Fact]
    public void Should_CreateNewOrder_Successfully()
    {
        // Act
        Result<Order> orderResult = Order.Create(
            _validCustomerId,
            _validTotalPrice,
            _validItems,
            _validAddress,
            _validSometimeUtc);

        // Assert
        orderResult.IsSuccess.Should().BeTrue();
        Order order = orderResult.Value;
        order.Id.Should().NotBeEmpty();
        order.CustomerId.Should().Be(_validCustomerId);
        order.TotalPrice.Should().Be(_validTotalPrice);
        order.Items.Should().BeEquivalentTo(_validItems);
        order.Address.Should().Be(_validAddress);
        order.Status.Should().Be(OrderStatus.Placed);
        order.CancelRequested.Should().BeFalse();
        order.CreatedOnUtc.Should().Be(_validSometimeUtc);
        order.ModifiedOnUtc.Should().Be(_validSometimeUtc);
        AssertDomainEventWasPublished<OrderCreatedDomainEvent>(order).OrderId.Should().Be(order.Id);
    }

    [Fact]
    public void Should_CreateNewOrderWithoutItems_Failure()
    {
        // Act
        Result<Order> orderResult = Order.Create(
            _validCustomerId,
            _validTotalPrice,
            [],
            _validAddress,
            _validSometimeUtc);

        // Assert
        orderResult.IsFailure.Should().BeTrue();
        orderResult.Error.Should().Be(OrderErrors.InvalidItems);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_CreateNewOrderWithInvalidQuantity_Failure(int invalidQuantity)
    {
        // Arrange
        List<DishItem> items =
        [
            new(Guid.NewGuid(), "Small", 2),
            new(Guid.NewGuid(),"Large", invalidQuantity)
        ];

        // Act
        Result<Order> orderResult = Order.Create(
            _validCustomerId,
            _validTotalPrice,
            items,
            _validAddress,
            _validSometimeUtc);

        // Assert
        orderResult.IsFailure.Should().BeTrue();
        orderResult.Error.Should().Be(OrderErrors.InvalidItems);
    }

    [Theory]
    [InlineData("", "Cluj-Napoca", "Strada Memorandumului", "28", "400114")]
    [InlineData("Cluj", "", "Strada Memorandumului", "28", "400114")]
    [InlineData("Cluj", "Cluj-Napoca", "", "28", "400114")]
    [InlineData("Cluj", "Cluj-Napoca", "Strada Memorandumului", "", "400114")]
    [InlineData("Cluj", "Cluj-Napoca", "Strada Memorandumului", "28", "")]
    [InlineData(" ", " ", " ", " ", " ")]
    public void Should_CreateNewOrderWithInvalidAddress_Failure(
        string county, string city, string street, string streetNumber, string zipCode)
    {
        // Arrange
        Address invalidAddress = new(county, city, street, streetNumber, zipCode);

        // Act
        Result<Order> orderResult = Order.Create(
            _validCustomerId,
            _validTotalPrice,
            _validItems,
            invalidAddress,
            _validSometimeUtc);

        // Assert
        orderResult.IsFailure.Should().BeTrue();
        orderResult.Error.Should().Be(OrderErrors.InvalidAddress);
    }
}
