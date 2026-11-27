using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Common.Tests.UnitTests;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Events;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;

namespace GoldenTable.Modules.Orders.Tests.UnitTests.Dishes.Create;

public sealed class CreateDish : BaseTest
{
    private readonly Money _validBasePrice;
    private readonly Name _validName;
    private readonly List<DishSize> _validSizes;
    private readonly DateTime _validSometimeUtc;

    public CreateDish()
    {
        _validName = new Name(Faker.Name.FullName());
        _validBasePrice = Money.Create(Faker.Random.Decimal(0M, 100M), "RON").Value;
        _validSizes = [new DishSize("Small", 100, 200), new DishSize("Medium", 200, 300)];
        _validSometimeUtc = DateTime.UtcNow;
    }

    [Fact]
    public void Should_CreateNewDish_Successfully()
    {
        // Act
        Result<Dish> dishResult = Dish.Create(_validName, _validBasePrice, _validSometimeUtc, _validSizes);

        // Assert
        dishResult.IsSuccess.Should().BeTrue();
        Dish dish = dishResult.Value;
        dish.Should().NotBeNull();
        dish.Id.Should().NotBeEmpty();
        dish.Name.Should().Be(_validName);
        dish.BasePrice.Should().Be(_validBasePrice);
        dish.Sizes.Should().BeEquivalentTo(_validSizes);
        dish.CreatedOnUtc.Should().Be(_validSometimeUtc);
        dish.ModifiedOnUtc.Should().Be(_validSometimeUtc);
        AssertDomainEventWasPublished<DishCreatedDomainEvent>(dish).DishId.Should().Be(dish.Id);
    }

    [Fact]
    public void Should_CreateNewDishWithoutSizes_Successfully()
    {
        // Act
        Result<Dish> dishResult = Dish.Create(_validName, _validBasePrice, _validSometimeUtc, []);

        // Assert
        dishResult.IsSuccess.Should().BeTrue();
        dishResult.Value.Sizes.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_CreateNewDishWithInvalidName_Failure(string invalidValue)
    {
        // Arrange
        Name invalidName = new(invalidValue);

        // Act
        Result<Dish> dishResult = Dish.Create(invalidName, _validBasePrice, _validSometimeUtc, _validSizes);

        // Assert
        dishResult.IsFailure.Should().BeTrue();
        dishResult.Error.Should().Be(DishErrors.InvalidName);
    }
}
