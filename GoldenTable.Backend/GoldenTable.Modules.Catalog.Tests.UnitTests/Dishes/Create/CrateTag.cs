using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes.Create;

public sealed class CrateTag : BaseTest
{
    [Fact]
    public void Should_CreateTag_Successfully()
    {
        // Arrange
        string name = Faker.Name.FirstName();

        // Act
        Result<DishTag> result = DishTag.Create(name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(name);
    }

    [Fact]
    public void Should_NotCreateTag_InvalidName()
    {
        // Arrange
        string name = string.Empty;

        // Act
        Result<DishTag> result = DishTag.Create(name);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
