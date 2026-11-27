using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class UpdateTagValue : BaseTest
{
    [Fact]
    public void Should_UpdateTagValue_Successfully()
    {
        // Arrange
        string originalValue = Faker.Name.FirstName();
        DishTag tag = DishTag.Create(originalValue).Value;
        string newValue = Faker.Name.LastName();

        // Act
        Result result = tag.UpdateValue(newValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        tag.Value.Should().Be(newValue);
    }

    [Fact]
    public void Should_NotUpdateTagValue_EmptyValue()
    {
        // Arrange
        string originalValue = Faker.Name.FirstName();
        DishTag tag = DishTag.Create(originalValue).Value;

        // Act
        Result result = tag.UpdateValue(string.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.InvalidValue);
        tag.Value.Should().Be(originalValue);
    }

    [Fact]
    public void Should_NotUpdateTagValue_NullValue()
    {
        // Arrange
        string originalValue = Faker.Name.FirstName();
        DishTag tag = DishTag.Create(originalValue).Value;

        // Act
        Result result = tag.UpdateValue(null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.InvalidValue);
        tag.Value.Should().Be(originalValue);
    }

    [Fact]
    public void Should_NotUpdateTagValue_SameValue()
    {
        // Arrange
        string originalValue = Faker.Name.FirstName();
        DishTag tag = DishTag.Create(originalValue).Value;

        // Act
        Result result = tag.UpdateValue(originalValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishTagErrors.SameValue);
        tag.Value.Should().Be(originalValue);
    }
}
