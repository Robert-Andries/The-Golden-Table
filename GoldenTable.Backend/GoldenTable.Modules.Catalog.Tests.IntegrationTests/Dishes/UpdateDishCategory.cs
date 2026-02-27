using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.UpdateDishCategory;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class UpdateDishCategory : DishesBaseTest
{
    public UpdateDishCategory(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_UpdatetDishCategory_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        string categoryName = Faker.Name.JobTitle();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new UpdateDishCategoryCommand(dish.Id, categoryName));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.Category));
        dbDish.Category.Name.Should().Be(categoryName);
    }

    [Fact]
    public async Task Should_NotUpdateDishCategory_DishNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var dishId = Guid.NewGuid();
        string categoryName = Faker.Name.JobTitle();

        // Act
        Result result = await Sender.Send(new UpdateDishCategoryCommand(dishId, categoryName));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }

    [Fact]
    public async Task Should_NotUpdateDishCategory_InvalidCategoryName()
    {
        // Arrange
        await ClearDatabaseAsync();
        string categoryName = "";
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new UpdateDishCategoryCommand(dish.Id, categoryName));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.InvalidCategoryName);
        dbDish.Should().BeEquivalentTo(dish);
    }

    [Fact]
    public async Task Should_NotUpdateDishCategory_SameAsBefore()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        string categoryName = dish.Category.Name;

        // Act
        Result result = await Sender.Send(new UpdateDishCategoryCommand(dish.Id, categoryName));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.SameCategory);
        dbDish.Should().BeEquivalentTo(dish);
    }
}
