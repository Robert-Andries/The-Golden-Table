using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class UpdateImages : DishBaseTest
{
    [Fact]
    public void Should_UpdateImages_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        var images = sut.Images.ToList();
        images.Add(Image.Create(SometimeUtc, new Uri("https://images.test/unique.png"), new Name("unique")).Value);

        // Act
        Result result = sut.UpdateImages(images, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Images)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Images.Select(i => i.Id).Should().BeEquivalentTo(images.Select(i => i.Id));
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedImagesDomainEvent>(sut).ImageIds.Should()
            .BeEquivalentTo(images.Select(i => i.Id));
    }

    [Fact]
    public void Should_NotRaiseEvent_SameImagesInDifferentOrder()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        var images = sut.Images.Reverse().ToList();

        // Act
        Result result = sut.UpdateImages(images, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotUpdateImages_DuplicateImages()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<Image> images = [sut.Images[0], sut.Images[0]];

        // Act
        Result result = sut.UpdateImages(images, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.ImagesAlreadyPresent);
        sut.Should().BeEquivalentTo(original);
    }
}
