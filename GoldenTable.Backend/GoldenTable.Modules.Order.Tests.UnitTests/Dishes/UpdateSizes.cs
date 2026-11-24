using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Order.Domain.Dishes;
using GoldenTable.Modules.Order.Domain.Dishes.Events;
using GoldenTable.Modules.Order.Domain.Dishes.ValueObjects;

namespace GoldenTable.Modules.Order.Tests.UnitTests.Dishes;

public sealed class UpdateSizes : DishBaseTest
{
    [Fact]
    public void Should_UpdateSizes_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishSize> sizes = [new("Unique small", 0, 150), new("Unique large", 10, 400)];

        // Act
        Result result = sut.UpdateSizes(sizes, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Sizes)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Sizes.Should().BeEquivalentTo(sizes);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        DishUpdatedSizesDomainEvent domainEvent = AssertDomainEventWasPublished<DishUpdatedSizesDomainEvent>(sut);
        domainEvent.DishId.Should().Be(sut.Id);
        domainEvent.Sizes.Should().BeEquivalentTo(sizes);
    }

    [Fact]
    public void Should_UpdateSizes_EmptyList()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishSize> sizes = [];

        // Act
        Result result = sut.UpdateSizes(sizes, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Sizes.Should().BeEmpty();
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedSizesDomainEvent>(sut).Sizes.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotRaiseEvent_SameSizesInDifferentOrder()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        var sizes = sut.Sizes.Reverse().ToList();

        // Act
        Result result = sut.UpdateSizes(sizes, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotUpdateSizes_DuplicateNames()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishSize> sizes = [new("Duplicate", 0, 150), new("Duplicate", 10, 400)];

        // Act
        Result result = sut.UpdateSizes(sizes, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.SizeAlreadyPresent);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Unique small", 0)]
    [InlineData("Unique small", -10)]
    [InlineData("", 150)]
    public void Should_NotUpdateSizes_InvalidSize(string name, decimal weight)
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishSize> sizes = [new(name, 0, weight)];

        // Act
        Result result = sut.UpdateSizes(sizes, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidSize);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }
}
