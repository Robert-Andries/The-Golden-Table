using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Events;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Tests.UnitTests.Orders;

public sealed class ModifyItems : OrderBaseTest
{
    [Fact]
    public void Should_ModifyItems_Successfully()
    {
        // Arrange
        Order original = OrderFaker.Generate();
        Order sut = original.DeepClone();
        List<DishItem> newItems =
        [
            new(Guid.NewGuid(), "Unique small", 1),
            new(Guid.NewGuid(), "Unique large", 3)
        ];
        Money newTotalPrice = Money.Create(Faker.Random.Decimal(1M, 500M), Currency.Eur).Value;

        // Act
        Result result = sut.ModifyItems(newItems, newTotalPrice, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(o => o.Items)
                .Excluding(o => o.TotalPrice)
                .Excluding(o => o.ModifiedOnUtc)
                .Excluding(o => o.DomainEvents);
        });
        sut.Items.Should().BeEquivalentTo(newItems);
        sut.TotalPrice.Should().Be(newTotalPrice);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        OrderItemsChangedDomainEvent domainEvent = AssertDomainEventWasPublished<OrderItemsChangedDomainEvent>(sut);
        domainEvent.OrderId.Should().Be(sut.Id);
        domainEvent.NewItems.Should().BeEquivalentTo(newItems);
    }

    [Fact]
    public void Should_NotModifyItems_EmptyItems()
    {
        // Arrange
        Order original = OrderFaker.Generate();
        Order sut = original.DeepClone();
        Money newTotalPrice = Money.Create(0M, Currency.Ron).Value;

        // Act
        Result result = sut.ModifyItems([], newTotalPrice, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidItems);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_NotModifyItems_InvalidQuantity(int invalidQuantity)
    {
        // Arrange
        Order original = OrderFaker.Generate();
        Order sut = original.DeepClone();
        List<DishItem> newItems =
        [
            new(Guid.NewGuid(), "Unique small", 1),
            new(Guid.NewGuid(), "Unique large", invalidQuantity)
        ];
        Money newTotalPrice = Money.Create(Faker.Random.Decimal(1M, 500M), Currency.Ron).Value;

        // Act
        Result result = sut.ModifyItems(newItems, newTotalPrice, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidItems);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }
}
