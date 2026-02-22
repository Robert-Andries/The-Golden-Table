using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Events;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Tests.Dishes.Create;

public class CreateImage : BaseTest
{
    [Fact]
    public void Should_CrateImage_Successfully()
    {
        // Arrange
        Name name = new(Faker.Name.FullName());
        Uri uri = new(Faker.Internet.Url());
        Description description = new(Faker.Name.JobDescriptor());

        // Act
        Result<Image> result = Image.Create(DateTime.MinValue, uri, name, description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.Uri.Should().Be(uri);
        AssertDomainEventWasPublished<ImageCreatedDomainEvent>(result.Value);
    }

    [Fact]
    public void Should_NotCrateImage_InvalidName()
    {
        // Arrange
        Name name = new("");
        Uri uri = new(Faker.Internet.Url());
        Description description = new(Faker.Name.JobDescriptor());

        // Act
        Result<Image> result = Image.Create(DateTime.MinValue, uri, name, description);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.InvalidName);
    }
}
