using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.UpdateNutritionalInformation;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.NutritionalValues;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class UpdateNutritionalInformation : DishesBaseTest
{
    public UpdateNutritionalInformation(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_UpdateNutritionalInformation_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        NutritionalValues nutritionalValues = NutritionalValues.Create(
            Faker.Random.Int(1, 800),
            Faker.Random.Int(1, 30),
            Faker.Random.Int(50, 100),
            Faker.Random.Int(0, 30),
            Faker.Random.Int(10, 80),
            Faker.Random.Int(10, 100)).Value;
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new UpdateNutritionalInformationCommand(dish.Id, nutritionalValues));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(x => x.NutritionalInformation));
        dbDish.NutritionalInformation.Should().BeEquivalentTo(nutritionalValues);
    }

    [Fact]
    public async Task Should_NotUpdateNutritionalInformation_DishDoesNotExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        var dishId = Guid.NewGuid();
        NutritionalValues nutritionalValues = NutritionalValues.Create(
            Faker.Random.Int(1, 800),
            Faker.Random.Int(1, 30),
            Faker.Random.Int(50, 100),
            Faker.Random.Int(0, 30),
            Faker.Random.Int(10, 80),
            Faker.Random.Int(10, 100)).Value;

        // Act
        Result result = await Sender.Send(new UpdateNutritionalInformationCommand(dishId, nutritionalValues));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }

    [Fact]
    public async Task Should_NotUpdateNutritionalInformation_SameNutritionalInformation()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        NutritionalValues nutritionalValues = dish.NutritionalInformation;

        // Act
        Result result = await Sender.Send(new UpdateNutritionalInformationCommand(dish.Id, nutritionalValues));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.NutritionalInformationIsTheSame);
        dbDish.Should().BeEquivalentTo(dish);
    }
}
