using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.Rename;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class Rename : DishesBaseTest
{
    public Rename(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Rename_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Name name = new(Faker.Name.FullName());
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new RenameCommand(dish.Id, name));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.Name));
        dbDish.Name.Should().Be(name);
    }

    [Fact]
    public async Task Should_NotRename_InvalidName()
    {
        // Arrange
        await ClearDatabaseAsync();
        Name name = new("");
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new RenameCommand(dish.Id, name));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.InvalidName);
        dbDish.Should().BeEquivalentTo(dish);
    }

    [Fact]
    public async Task Should_Rename_SameNameAsBefore()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        Name name = dish.Name;

        // Act
        Result result = await Sender.Send(new RenameCommand(dish.Id, name));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.SameName);
        dbDish.Should().BeEquivalentTo(dish);
    }
}
