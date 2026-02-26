using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Events;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Images;

public sealed class UpdateUri : ImageBaseTest
{
    [Fact]
    public void Should_UpdateUri_Successfully()
    {
        // Arrange
        Image original = ImageFaker.Generate();
        Image sut = original.DeepClone();
        Uri uri = new(Faker.Internet.Url());

        // Act
        Result result = sut.UpdateUri(uri, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(i => i.Uri)
                .Excluding(i => i.DomainEvents);
        });
        sut.Uri.Should().Be(uri);
        AssertDomainEventWasPublished<ImageUriUpdatedDomainEvent>(sut);
    }
    
    [Fact]
    public void Should_NotUpdateUri_InvalidUri()
    {
        // Arrange
        Image original = ImageFaker.Generate();
        Image sut = original.DeepClone();
        Uri uri = new("https://test.testing.ro/{id}/"); // {} are not allowed

        // Act
        Result result = sut.UpdateUri(uri, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
    }
}
