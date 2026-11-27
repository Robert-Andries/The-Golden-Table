using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Events;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Images;

public sealed class Create : BaseTest
{
    [Fact]
    public void Should_CreateImage_Successfully()
    {
        // Arrange
        Name name = new(Faker.Name.FullName());
        Description description = new(Faker.Name.JobDescriptor());
        Uri uri = new(Faker.Internet.Url());
        DateTime date = Faker.Date.Recent();

        // Act
        Result<Image> result = Image.Create(date, uri, name, description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Uri.Should().Be(uri);
        result.Value.Description.Should().Be(description);
        AssertDomainEventWasPublished<ImageCreatedDomainEvent>(result.Value);
    }

    [Fact]
    public void Should_NotCreateImage_InvalidName()
    {
        // Arrange
        Name name = new("");
        Description description = new(Faker.Name.JobDescriptor());
        Uri uri = new(Faker.Internet.Url());
        DateTime date = Faker.Date.Recent();

        // Act
        Result<Image> result = Image.Create(date, uri, name, description);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.InvalidName);
    }

    [Fact]
    public void Should_NotCreateImage_InvalidUri()
    {
        // Arrange
        Name name = new(Faker.Name.FullName());
        Description description = new(Faker.Name.JobDescriptor());
        Uri uri = new("https://testing.test.com/{id}"); // {} are not allowed
        DateTime date = Faker.Date.Recent();

        // Act
        Result<Image> result = Image.Create(date, uri, name, description);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ImageErrors.InvalidUri);
    }
}
