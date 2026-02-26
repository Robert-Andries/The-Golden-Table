using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Events;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Images;

public sealed class Rename : ImageBaseTest
{
    [Fact]
    public void Should_RenameImage_Successfully()
    {
        // Arrange
        Image original = ImageFaker.Generate();
        Image sut = original.DeepClone();
        Name name = new(Faker.Name.FullName());

        // Act
        Result result = sut.Rename(name, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(i => i.Name)
                .Excluding(i => i.DomainEvents);
        });
        sut.Name.Should().Be(name);
        AssertDomainEventWasPublished<ImageRenamedDomainEvent>(sut);
    }
    
    [Fact]
    public void Should_NotRenameImage_InvalidName()
    {
        // Arrange
        Image original = ImageFaker.Generate();
        Image sut = original.DeepClone();
        Name name = new("");

        // Act
        Result result = sut.Rename(name, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.InvalidName);
        sut.Should().BeEquivalentTo(original);
    }
    
    [Fact]
    public void Should_RenameImage_SameName()
    {
        // Arrange
        Image original = ImageFaker.Generate();
        Image sut = original.DeepClone();
        Name name = sut.Name;

        // Act
        Result result = sut.Rename(name, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
    }
}
