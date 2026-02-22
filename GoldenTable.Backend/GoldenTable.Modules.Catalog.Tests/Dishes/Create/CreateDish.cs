using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Tests.Dishes.Create;

public sealed class CreateDish : BaseTest
{
    private readonly Money _validBasePrice;
    private readonly DishCategory _validCategory;
    private readonly Description _validDescription;

    private readonly Name _validName;
    private readonly NutritionalValues _validNutritionalValues;
    private readonly List<DishSize> _validSizes;
    private readonly DateTime _validSometimeUtc;
    private readonly List<DishTag> _validTags;

    public CreateDish()
    {
        _validName = new Name(Faker.Name.FullName());
        _validDescription = new Description(Faker.Hacker.Phrase());
        _validBasePrice = Money.Create(Faker.Random.Decimal(0M, 100M), "RON").Value;
        _validSizes = [new DishSize("Small", 100f, 200), new DishSize("Medium", 200f, 300f)];
        _validNutritionalValues = NutritionalValues.Create(200, 200, 200, 200, 200, 200).Value;
        _validCategory = DishCategory.Create(Faker.Name.LastName()).Value;
        _validTags = [DishTag.Create("Vegan").Value];
        _validSometimeUtc = DateTime.UtcNow;
    }

    [Fact]
    public void Should_CreateNewDish_Successfully()
    {
        // Act
        Dish dish = Dish.Create(
            _validName,
            _validDescription,
            _validBasePrice,
            _validSizes,
            _validNutritionalValues,
            _validCategory,
            _validTags,
            _validSometimeUtc).Value;

        // Assert
        dish.Should().NotBeNull();
        dish.Name.Should().Be(_validName);
        dish.Description.Should().Be(_validDescription);
        dish.BasePrice.Should().Be(_validBasePrice);
        dish.Sizes.Should().BeEquivalentTo(_validSizes);
        dish.NutritionalInformation.Should().Be(_validNutritionalValues);
        dish.Category.Should().Be(_validCategory);
        dish.Tags.Should().BeEquivalentTo(_validTags);
        dish.CreatedOnUtc.Should().Be(_validSometimeUtc);
        dish.ModifiedOnUtc.Should().Be(_validSometimeUtc);
        AssertDomainEventWasPublished<DishCreatedDomainEvent>(dish).DishId.Should().Be(dish.Id);
    }

    [Fact]
    public void Should_CreateNewDishWithInvalidName_Failure()
    {
        // Arrange
        Name invalidName = new("");

        // Act
        Result<Dish> dishResult = Dish.Create(
            invalidName,
            _validDescription,
            _validBasePrice,
            _validSizes,
            _validNutritionalValues,
            _validCategory,
            _validTags,
            _validSometimeUtc);

        dishResult.IsFailure.Should().BeTrue();
        dishResult.Error.Should().Be(DishErrors.InvalidName);
    }

    [Fact]
    public void Should_CreateNewDishWithInvalidDescription_Failure()
    {
        // Arrange
        Description invalidDescription = new("");

        // Act
        Result<Dish> dishResult = Dish.Create(
            _validName,
            invalidDescription,
            _validBasePrice,
            _validSizes,
            _validNutritionalValues,
            _validCategory,
            _validTags,
            _validSometimeUtc);

        dishResult.IsFailure.Should().BeTrue();
        dishResult.Error.Should().Be(DishErrors.InvalidDescription);
    }
}
