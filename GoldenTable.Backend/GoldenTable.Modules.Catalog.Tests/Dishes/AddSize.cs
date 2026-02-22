using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Tests.Dishes;

public sealed class AddSize : DishBaseTest
{
    [Fact]
    public void Should_AddSize_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        DishSize size = new("XXLLXL", 100, 200);

        // Act
        Result result = sut.AddSize(size, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.DomainEvents)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.Sizes);
        });
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        sut.Sizes.Count.Should().Be(original.Sizes.Count + 1);
        foreach (DishSize originalSize in original.Sizes)
        {
            sut.Sizes.Should().Contain(originalSize);
        }

        AssertDomainEventWasPublished<DishUpdatedSizeDomainEvent>(sut);
    }

    [Fact]
    public void Should_FailAddSize_WeightIsInvalid()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        DishSize size = new("XXLLXL", 100, 0);

        // Act
        Result result = sut.AddSize(size, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidSize);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Any(de => de is DishUpdatedSizeDomainEvent).Should().BeFalse();
    }

    [Fact]
    public void Should_FailAddSize_NameIsInvalid()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        DishSize size = new("", 100, 320);

        // Act
        Result result = sut.AddSize(size, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidSize);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Any(de => de is DishUpdatedSizeDomainEvent).Should().BeFalse();
    }
}
