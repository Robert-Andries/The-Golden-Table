using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.UpdateDescription;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class UpdateDescription : DishesBaseTest
{
    public UpdateDescription(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_UpdateDescription_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        string description = Faker.Name.JobDescriptor();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new UpdateDescriptionCommand(dish.Id, description));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.Description));
        dbDish.Description.Value.Should().Be(description);
    }

    [Fact]
    public async Task Should_NotUpdateDescription_InvalidDescription()
    {
        // Arrange
        await ClearDatabaseAsync();
        string description = "";
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new UpdateDescriptionCommand(dish.Id, description));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.InvalidDescription);
        dbDish.Should().BeEquivalentTo(dish);
    }

    [Fact]
    public async Task Should_UpdateDescription_SameDescription_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        string description = dish.Description.Value;

        // Act
        Result result = await Sender.Send(new UpdateDescriptionCommand(dish.Id, description));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.SameDescription);
        dbDish.Should().BeEquivalentTo(dish);
    }

    [Fact]
    public async Task Should_NotUpdateDescription_DishNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var dishId = Guid.NewGuid();
        string description = Faker.Name.JobDescriptor();

        // Act
        Result result = await Sender.Send(new UpdateDescriptionCommand(dishId, description));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }
}
