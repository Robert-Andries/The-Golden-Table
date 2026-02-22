using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;

namespace GoldenTable.Modules.Catalog.Tests.Dishes;

public sealed class AddImage : DishBaseTest
{
    [Fact]
    public void Should_AddImage_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Name imageName = new(Faker.Name.FullName());
        Uri imageUri = new(Faker.Internet.Url());
        Image image = Image.Create(SometimeUtc, imageUri, imageName).Value;

        // Act
        Result result = sut.AddImage(image, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Images)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        foreach (Image originalImage in original.Images)
        {
            sut.Images.Any(i => i.Id == originalImage.Id).Should().BeTrue();
        }
        sut.Images.Count.Should().Be(original.Images.Count + 1);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedImagesDomainEvent>(sut);
    }

    [Fact]
    public void Should_NotAddImage_ImageAlreadyExists()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Name imageName = new(Faker.Name.FullName());
        Uri imageUri = new(Faker.Internet.Url());
        Image image = Image.Create(SometimeUtc, imageUri, imageName).Value;
        original.AddImage(image, SometimeUtc);
        Dish sut = original.DeepClone();

        // Act
        Result result = sut.AddImage(image, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.ImageAlreadyPresent);
        sut.Should().BeEquivalentTo(original);
    }
}
