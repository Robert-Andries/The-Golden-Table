using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;

namespace GoldenTable.Modules.Catalog.Tests.Dishes;

public class RemoveImage : DishBaseTest
{
    [Fact]
    public void Should_RemoveImage_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Image image = Faker.PickRandom(original.Images, 1).First();
        original.AddImage(image, SometimeUtc);
        Dish sut = original.DeepClone();

        // Act
        Result result = sut.RemoveImage(image.Id, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Images)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Images.Count.Should().Be(original.Images.Count - 1);
        foreach (Image originalImage in original.Images)
        {
            if (originalImage.Id == image.Id)
            {
                continue;
            }
            sut.Images.Any(i => i.Id == originalImage.Id).Should().BeTrue();
        }

        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedImagesDomainEvent>(sut);
    }

    [Fact]
    public void Should_NotRemoveImage_ImageNotFound()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Image randomImage = Faker.PickRandom(original.Images, 1).First();
        foreach (Image originalImage in original.Images)
        {
            original.RemoveImage(originalImage.Id, SometimeUtc);
        }

        Dish sut = original.DeepClone();

        // Act
        Result result = sut.RemoveImage(randomImage.Id, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.ImageNotPresent);
        sut.Should().BeEquivalentTo(original);
    }
}
