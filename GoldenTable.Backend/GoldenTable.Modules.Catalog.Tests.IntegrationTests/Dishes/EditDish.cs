using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class EditDish : DishesBaseTest
{
    public EditDish(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_EditDish_WhenOnlyNameChanges()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.WithSizes([new DishSize("Small", 0, 200), new DishSize("Large", 5, 400)]).Build();
        await PutDishInDb(dish);
        string newName = Faker.Name.FullName() + Guid.NewGuid().ToString("N");

        // Act
        Result result = await Sender.Send(CreateEditDishCommand(dish, name: newName));

        // Assert
        result.IsSuccess.Should().BeTrue();
        Dish dbDish = await context.Dishes.AsNoTracking().FirstAsync(d => d.Id == dish.Id);
        dbDish.Name.Value.Should().Be(newName);
    }

    [Fact]
    public async Task Should_EditDish_WhenSizesAndTagsChange()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.WithSizes([new DishSize("Small", 0, 200), new DishSize("Large", 5, 400)]).Build();
        await PutDishInDb(dish);
        List<DishSize> sizes = [new("Medium", 2, 300)];
        var tagIds = dish.Tags.Skip(1).Select(t => t.Id).ToList();

        // Act
        Result result = await Sender.Send(CreateEditDishCommand(dish, sizes: sizes, tagIds: tagIds));

        // Assert
        result.IsSuccess.Should().BeTrue();
        Dish dbDish = await context.Dishes
            .AsNoTracking()
            .Include(d => d.Tags)
            .FirstAsync(d => d.Id == dish.Id);
        dbDish.Sizes.Should().BeEquivalentTo(sizes);
        dbDish.Tags.Select(t => t.Id).Should().BeEquivalentTo(tagIds);
    }

    [Fact]
    public async Task Should_NotEditDish_DishDoesNotExist()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();

        // Act
        Result result = await Sender.Send(CreateEditDishCommand(dish));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }

    [Fact]
    public async Task Should_NotEditDish_NameBelongsToAnotherDish()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        Dish otherDish = DishBuilder.Build();
        await PutDishInDb(dish);
        await PutDishInDb(otherDish);

        // Act
        Result result = await Sender.Send(CreateEditDishCommand(dish, name: otherDish.Name.Value));

        // Assert
        result.Error.Should().Be(DishErrors.DishAlreadyExists);
    }
}
