using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.RemoveSize;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class RemoveSize : DishesBaseTest
{
    public RemoveSize(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_RemoveSize_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishSize size = new(Faker.Name.FullName(), Faker.Random.Float(1F, 100F), Faker.Random.Float(1F, 100F));
        DishSize size2 = new(Faker.Name.FullName(), Faker.Random.Float(1F, 100F), Faker.Random.Float(1F, 100F));
        Dish dish = DishBuilder.WithSizes([size, size2]).Build();
        await PutDishInDb(dish);
        context.ChangeTracker.Clear();

        // Act
        Result result = await Sender.Send(new RemoveSizeCommand(dish.Id, size.Name));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.Sizes));
        dbDish.Sizes.Count.Should().Be(dish.Sizes.Count - 1);
        foreach (DishSize dishSize in dish.Sizes.Where(s => s != size).ToList())
        {
            dbDish.Sizes.Contains(dishSize).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Should_NotRemoveSize_DishNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var dishId = Guid.NewGuid();

        // Act
        Result result = await Sender.Send(new RemoveSizeCommand(dishId, Faker.Name.FullName()));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }

    [Fact]
    public async Task Should_NotRemoveSize_SizeNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new RemoveSizeCommand(dish.Id, Faker.Name.JobTitle()));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.SizeDoesNotExist);
        dbDish.Should().BeEquivalentTo(dish);
    }
}
