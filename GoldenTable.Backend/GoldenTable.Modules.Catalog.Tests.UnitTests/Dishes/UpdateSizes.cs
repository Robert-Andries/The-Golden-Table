using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

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
        AssertDomainEventWasPublished<DishUpdatedSizesDomainEvent>(sut).Sizes.Should().BeEquivalentTo(sizes);
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
    }

    [Fact]
    public void Should_NotUpdateSizes_InvalidSize()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishSize> sizes = [new("Unique small", 0, 0)];

        // Act
        Result result = sut.UpdateSizes(sizes, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidSize);
        sut.Should().BeEquivalentTo(original);
    }
}
