using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;

namespace GoldenTable.Modules.Catalog.Tests.Dishes.Create;

public sealed class CreateNutritionalValue : BaseTest
{
    [Fact]
    public void Should_CreateNutritionalValue_Successfully()
    {
        // Arrange
        float kcal = Faker.Random.Number(200);
        float gramsOfFat = Faker.Random.Number(200);
        float gramsOfCarbohydrates = Faker.Random.Number(100, 200);
        float gramsOfSugar = Faker.Random.Number(100);
        float gramsOfProtein = Faker.Random.Number(200);
        float gramsOfSalt = Faker.Random.Number(100);
        Carbohydrates carbohydrates = Carbohydrates.Create(gramsOfCarbohydrates, gramsOfSugar).Value;
        Energy energy = Energy.Create(kcal).Value;

        // Act
        Result<NutritionalValues> result = NutritionalValues.Create(kcal, gramsOfFat, gramsOfCarbohydrates,
            gramsOfSugar, gramsOfProtein,
            gramsOfSalt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Energy.Should().Be(energy);
        result.Value.GramsOfFat.Should().Be(gramsOfFat);
        result.Value.GramsOfCarbohydrates.Should().Be(carbohydrates);
        result.Value.GramsOfProtein.Should().Be(gramsOfProtein);
        result.Value.GramsOfSalt.Should().Be(gramsOfSalt);
    }

    [Fact]
    public void Should_NotCreateNutritionalValue_KcalIsInvalid()
    {
        // Arrange
        float kcal = -1;
        float gramsOfFat = Faker.Random.Number(100);
        float gramsOfCarbohydrates = Faker.Random.Number(50, 100);
        float gramsOfSugar = Faker.Random.Number(50);
        float gramsOfProtein = Faker.Random.Number(100);
        float gramsOfSalt = Faker.Random.Number(100);

        // Act
        Result<NutritionalValues> result = NutritionalValues.Create(kcal, gramsOfFat, gramsOfCarbohydrates,
            gramsOfSugar, gramsOfProtein,
            gramsOfSalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NutritionalValuesErrors.InvalidKcal);
    }

    [Fact]
    public void Should_NotCreateNutritionalValue_GramsOfSugarIsBiggerThenGramsOfCarbohydrates()
    {
        // Arrange
        float kcal = Faker.Random.Number(200);
        float gramsOfFat = Faker.Random.Number(200);
        float gramsOfCarbohydrates = Faker.Random.Number(200);
        float gramsOfSugar = Faker.Random.Number(201, 1000);
        float gramsOfProtein = Faker.Random.Number(200);
        float gramsOfSalt = Faker.Random.Number(100);

        // Act
        Result<NutritionalValues> result = NutritionalValues.Create(kcal, gramsOfFat, gramsOfCarbohydrates,
            gramsOfSugar, gramsOfProtein,
            gramsOfSalt);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(NutritionalValuesErrors.GramsOfSugarShouldNotExceedGramsOfCarbohydrates);
    }
}
