using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class RemoveTags : DishBaseTest
{
    [Fact]
    public void Should_RemoveTags_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        var tags = Faker.PickRandom(original.Tags, Faker.Random.Int(1, original.Tags.Count)).ToList();
        var remainingTags = original.Tags.Except(tags).ToList();

        // Act
        Result result = sut.RemoveTags(tags, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Tags)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Tags.Count.Should().Be(remainingTags.Count);
        foreach (DishTag dishTag in tags)
        {
            sut.Tags.Should().NotContain(dishTag);
        }

        foreach (DishTag dishTag in remainingTags)
        {
            sut.Tags.Any(t => t.Id == dishTag.Id).Should().BeTrue();
        }

        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedTagsDomainEvent>(sut);
    }

    [Fact]
    public void Should_NotRemoveTags_TagsAreEmpty()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishTag> emtpyTags = new();

        // Act
        Result result = sut.RemoveTags(emtpyTags, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidTags);
        sut.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Should_NotRemoveTags_TagsAreNotPartOfCollection()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        List<DishTag> emtpyTags = new();
        emtpyTags.Add(DishTag.Create("Some unique value 1").Value);
        emtpyTags.Add(DishTag.Create("Some unique value 2").Value);
        emtpyTags.Add(DishTag.Create("Some unique value 3").Value);

        // Act
        Result result = sut.RemoveTags(emtpyTags, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
    }
}
