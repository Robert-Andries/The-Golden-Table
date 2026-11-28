using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class UpdateTags : DishBaseTest
{
    [Fact]
    public void Should_UpdateTags_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishTag> tags =
        [
            sut.Tags[0],
            DishTag.Create("Some random unique tag 1").Value,
            DishTag.Create("Some random unique tag 2").Value
        ];

        // Act
        Result result = sut.UpdateTags(tags, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Tags)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Tags.Should().BeEquivalentTo(tags);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedTagsDomainEvent>(sut).TagIds.Should()
            .BeEquivalentTo(tags.Select(t => t.Id));
    }

    [Fact]
    public void Should_UpdateTags_WhenTagsAreRemoved()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        var tags = sut.Tags.Skip(1).ToList();

        // Act
        Result result = sut.UpdateTags(tags, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Tags.Should().BeEquivalentTo(tags);
        AssertDomainEventWasPublished<DishUpdatedTagsDomainEvent>(sut);
    }

    [Fact]
    public void Should_NotRaiseEvent_SameTagsInDifferentOrder()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        var tags = sut.Tags.Reverse().ToList();

        // Act
        Result result = sut.UpdateTags(tags, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotUpdateTags_DuplicateTags()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        DishTag tag = DishTag.Create("Some random unique tag 1").Value;
        List<DishTag> tags = [tag, tag];

        // Act
        Result result = sut.UpdateTags(tags, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.TagsAlreadyPresent);
        sut.Should().BeEquivalentTo(original);
    }
}
