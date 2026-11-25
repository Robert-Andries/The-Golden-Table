using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Events;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Tests.UnitTests.Orders;

public sealed class RequestCancel : OrderBaseTest
{
    [Fact]
    public void Should_RequestCancel_Successfully()
    {
        // Arrange
        Order original = OrderFaker.Generate();
        Order sut = original.DeepClone();

        // Act
        Result result = sut.RequestCancel(SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(o => o.CancelRequested)
                .Excluding(o => o.ModifiedOnUtc)
                .Excluding(o => o.DomainEvents);
        });
        sut.CancelRequested.Should().BeTrue();
        sut.Status.Should().Be(original.Status);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<OrderCancelRequestedDomainEvent>(sut).OrderId.Should().Be(sut.Id);
    }

    [Fact]
    public void Should_NotRequestCancel_AlreadyRequested()
    {
        // Arrange
        Order original = OrderFaker.Generate();
        original.RequestCancel(SometimeUtc.AddHours(-1));
        original.ClearDomainEvents();
        Order sut = original.DeepClone();

        // Act
        Result result = sut.RequestCancel(SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CancelAlreadyRequested);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Preparing)]
    public void Should_RequestCancel_WhenOrderIsInProgress(OrderStatus status)
    {
        // Arrange
        Order sut = MoveToStatus(OrderFaker.Generate(), status);

        // Act
        Result result = sut.RequestCancel(SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.CancelRequested.Should().BeTrue();
        sut.Status.Should().Be(status);
    }

    [Theory]
    [InlineData(OrderStatus.Sent)]
    [InlineData(OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Rejected)]
    public void Should_NotRequestCancel_OrderIsFinished(OrderStatus status)
    {
        // Arrange
        Order original = MoveToStatus(OrderFaker.Generate(), status);
        Order sut = original.DeepClone();

        // Act
        Result result = sut.RequestCancel(SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CannotRequestCancel);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }
}
