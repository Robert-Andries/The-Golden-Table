using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Events;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Images;

public sealed class UpdateDescription : ImageBaseTest
{
    [Fact]
    public void Should_UpdateDescription_Successfully()
    {
        // Arrange
        Image original = ImageFaker.Generate();
        Image sut = original.DeepClone();
        Description description = new(Faker.Name.JobDescriptor());

        // Act
        Result result = sut.UpdateDescription(description, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, opts =>
        {
            return opts.Excluding(x => x.Description)
                .Excluding(i => i.DomainEvents);
        });
        sut.Description.Should().Be(description);
        AssertDomainEventWasPublished<ImageDescriptionUpdatedDomainEvent>(sut);
    }
    
    [Fact]
    public void Should_UpdateDescription_Successfully_SameName()
    {
        // Arrange
        Image original = ImageFaker.Generate();
        Image sut = original.DeepClone();
        Description? description = sut.Description;

        // Act
        Result result = sut.UpdateDescription(description, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
    }
}
