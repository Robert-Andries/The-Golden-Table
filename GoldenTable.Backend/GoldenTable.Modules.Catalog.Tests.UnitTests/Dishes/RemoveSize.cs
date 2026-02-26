using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class RemoveSize : DishBaseTest
{
    [Fact]
    public void Should_RemoveSize_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        DishSize validSize = Faker.PickRandom(sut.Sizes, 1).First();

        // Act
        Result result = sut.RemoveSize(validSize.Name, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Sizes)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Sizes.Count.Should().Be(original.Sizes.Count - 1);
        foreach (DishSize originalSize in original.Sizes)
        {
            if (originalSize == validSize)
            {
                continue;
            }

            sut.Sizes.Should().Contain(originalSize);
        }

        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedSizeDomainEvent>(sut);
    }

    [Fact]
    public void Should_NotRemoveSize_InvalidDishSize()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        DishSize invalidSize = new("invalid", 0, 0);

        // Act
        Result result = sut.RemoveSize(invalidSize.Name, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.SizeDoesNotExist);
        sut.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Should_NotRemoveSize_SizeCollectionIsEmpty()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        foreach (DishSize originalSize in original.Sizes)
        {
            original.RemoveSize(originalSize.Name, SometimeUtc);
        }

        original.ClearDomainEvents();
        Dish sut = original.DeepClone();
        DishSize invalidSize = new("invalid", 0, 0);

        // Act
        Result result = sut.RemoveSize(invalidSize.Name, SometimeUtc);

        // Assert
        original.Sizes.Count.Should().Be(0);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.SizeDoesNotExist);
        sut.Should().BeEquivalentTo(original);
    }
}
