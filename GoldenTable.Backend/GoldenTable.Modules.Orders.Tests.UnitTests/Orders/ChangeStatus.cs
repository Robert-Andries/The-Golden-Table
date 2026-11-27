using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Orders.Domain.Orders;
using GoldenTable.Modules.Orders.Domain.Orders.Events;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Tests.UnitTests.Orders;

public sealed class ChangeStatus : OrderBaseTest
{
    private static readonly (OrderStatus From, OrderStatus To)[] Allowed =
    [
        (OrderStatus.Placed, OrderStatus.Confirmed),
        (OrderStatus.Placed, OrderStatus.Rejected),
        (OrderStatus.Placed, OrderStatus.Cancelled),
        (OrderStatus.Confirmed, OrderStatus.Preparing),
        (OrderStatus.Confirmed, OrderStatus.Cancelled),
        (OrderStatus.Preparing, OrderStatus.Sent),
        (OrderStatus.Preparing, OrderStatus.Cancelled)
    ];

    public static TheoryData<OrderStatus, OrderStatus> AllowedTransitions
    {
        get
        {
            TheoryData<OrderStatus, OrderStatus> data = new();
            foreach ((OrderStatus from, OrderStatus to) in Allowed)
            {
                data.Add(from, to);
            }

            return data;
        }
    }

    public static TheoryData<OrderStatus, OrderStatus> InvalidTransitions
    {
        get
        {
            TheoryData<OrderStatus, OrderStatus> data = new();
            foreach (OrderStatus from in Enum.GetValues<OrderStatus>())
            {
                foreach (OrderStatus to in Enum.GetValues<OrderStatus>())
                {
                    if (from != to && !Allowed.Contains((from, to)))
                    {
                        data.Add(from, to);
                    }
                }
            }

            return data;
        }
    }

    public static TheoryData<OrderStatus> AllStatuses => new(Enum.GetValues<OrderStatus>());

    [Theory]
    [MemberData(nameof(AllowedTransitions))]
    public void Should_ChangeStatus_Successfully(OrderStatus from, OrderStatus to)
    {
        // Arrange
        Order original = MoveToStatus(OrderFaker.Generate(), from);
        Order sut = original.DeepClone();

        // Act
        Result result = sut.UpdateStatus(to, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(o => o.Status)
                .Excluding(o => o.ModifiedOnUtc)
                .Excluding(o => o.DomainEvents);
        });
        sut.Status.Should().Be(to);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        OrderStatusChangedDomainEvent domainEvent = AssertDomainEventWasPublished<OrderStatusChangedDomainEvent>(sut);
        domainEvent.OrderId.Should().Be(sut.Id);
        domainEvent.NewStatus.Should().Be(to);
    }

    [Theory]
    [MemberData(nameof(InvalidTransitions))]
    public void Should_NotChangeStatus_InvalidTransition(OrderStatus from, OrderStatus to)
    {
        // Arrange
        Order original = MoveToStatus(OrderFaker.Generate(), from);
        Order sut = original.DeepClone();

        // Act
        Result result = sut.UpdateStatus(to, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidStatus);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(AllStatuses))]
    public void Should_NotRaiseEvent_SameStatus(OrderStatus status)
    {
        // Arrange
        Order original = MoveToStatus(OrderFaker.Generate(), status);
        Order sut = original.DeepClone();

        // Act
        Result result = sut.UpdateStatus(status, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }
}
