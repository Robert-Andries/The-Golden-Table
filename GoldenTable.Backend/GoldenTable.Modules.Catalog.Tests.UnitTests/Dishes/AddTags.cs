using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class AddTags : DishBaseTest
{
    [Fact]
    public void Should_AddTags_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishTag> tags = new();
        tags.Add(DishTag.Create("Some random unique tag 1").Value);
        tags.Add(DishTag.Create("Some random unique tag 2").Value);
        tags.Add(DishTag.Create("Some random unique tag 3").Value);

        // Act
        Result result = sut.AddTags(tags, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Tags)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Tags.Count.Should().Be(original.Tags.Count + tags.Count);
        foreach (DishTag tag in original.Tags)
        {
            sut.Tags.Any(t => t.Id == tag.Id && t.Value == tag.Value).Should().BeTrue();
        }

        foreach (DishTag tag in tags)
        {
            sut.Tags.Should().Contain(tag);
        }

        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedTagsDomainEvent>(sut);
    }

    [Fact]
    public void Should_NotAddTags_ProvidedTagsAreEmptyList()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishTag> tags = new();

        // Act
        Result result = sut.AddTags(tags, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidTags);
        sut.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Should_NotAddTags_ProvidedTagsSameAsDishTags()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        var tags = sut.Tags.ToList();

        // Act
        Result result = sut.AddTags(tags, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.TagsAlreadyPresent);
        sut.Should().BeEquivalentTo(original);
    }
}
