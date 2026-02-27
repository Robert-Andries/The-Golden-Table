using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class UpdateDishCategory : DishBaseTest
{
    [Fact]
    public void Should_UpdateDishCategory_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        DishCategory category = DishCategory.Create(Faker.Name.FirstName()).Value;

        // Act
        Result result = sut.UpdateDishCategory(category, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.Category)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.Category.Should().Be(category);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedCategoryDomainEvent>(sut);
    }

    [Fact]
    public void Should_NotUpdateDishCategory_SameCategory()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        DishCategory sameCategory = sut.Category;

        // Act
        Result result = sut.UpdateDishCategory(sameCategory, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DishErrors.SameCategory);
        sut.Should().BeEquivalentTo(original);
    }
}
