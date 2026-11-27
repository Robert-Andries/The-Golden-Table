using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes.Create;

public sealed class CreateCategory : BaseTest
{
    [Fact]
    public void Should_CreateCategory_Successfully()
    {
        // Arrange
        string name = Faker.Name.FullName();

        // Act
        Result<DishCategory> result = DishCategory.Create(name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
    }

    [Fact]
    public void Should_NotCreateCategory_InvalidName()
    {
        // Arrange
        string name = "";

        // Act
        Result<DishCategory> result = DishCategory.Create(name);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.InvalidCategoryName);
    }
}
