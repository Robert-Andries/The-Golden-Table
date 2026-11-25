using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Order.Domain.Orders;
using GoldenTable.Modules.Order.Domain.Orders.Events;
using GoldenTable.Modules.Order.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Order.Tests.UnitTests.Orders;

public sealed class UpdateAddress : OrderBaseTest
{
    [Fact]
    public void Should_UpdateAddress_Successfully()
    {
        // Arrange
        Domain.Orders.Order original = OrderFaker.Generate();
        Domain.Orders.Order sut = original.DeepClone();
        Address newAddress = new("Unique county", "Unique city", "Unique street", "1A", "000000");

        // Act
        Result result = sut.UpdateAddress(newAddress, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(o => o.Address)
                .Excluding(o => o.ModifiedOnUtc)
                .Excluding(o => o.DomainEvents);
        });
        sut.Address.Should().Be(newAddress);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        sut.ModifiedOnUtc.Should().NotBe(sut.CreatedOnUtc);
        OrderAddressChangedDomainEvent domainEvent = AssertDomainEventWasPublished<OrderAddressChangedDomainEvent>(sut);
        domainEvent.OrderId.Should().Be(sut.Id);
        domainEvent.NewAddress.Should().Be(newAddress);
    }

    [Theory]
    [InlineData("", "City", "Street", "1", "000000")]
    [InlineData("County", "", "Street", "1", "000000")]
    [InlineData("County", "City", "", "1", "000000")]
    [InlineData("County", "City", "Street", "", "000000")]
    [InlineData("County", "City", "Street", "1", "")]
    [InlineData(" ", " ", " ", " ", " ")]
    public void Should_NotUpdateAddress_InvalidAddress(
        string county, string city, string street, string streetNumber, string zipCode)
    {
        // Arrange
        Domain.Orders.Order original = OrderFaker.Generate();
        Domain.Orders.Order sut = original.DeepClone();
        Address invalidAddress = new(county, city, street, streetNumber, zipCode);

        // Act
        Result result = sut.UpdateAddress(invalidAddress, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidAddress);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotUpdateAddress_SameAddress()
    {
        // Arrange
        Domain.Orders.Order original = OrderFaker.Generate();
        Domain.Orders.Order sut = original.DeepClone();
        Address sameAddress = sut.Address with { };

        // Act
        Result result = sut.UpdateAddress(sameAddress, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.SameAddress);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }
}
