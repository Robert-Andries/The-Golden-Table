using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.AddTags;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class AddTags : DishesBaseTest
{
    public AddTags(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_AddTags_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        List<string> dishTags = new();
        int numberOfTags = Faker.Random.Int(1, 4);
        while (numberOfTags-- > 0)
        {
            dishTags.Add(Faker.Name.FullName());
        }

        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        context.ChangeTracker.Clear();

        // Act
        Result result = await Sender.Send(new AddTagsCommand(dish.Id, dishTags));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.Tags));
        dbDish.Tags.Count.Should().Be(dish.Tags.Count + dishTags.Count);
        foreach (string tag in dishTags)
        {
            dbDish.Tags.Any(t => t.Value == tag).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Should_AddTags_OverlapSomeTags()
    {
        // Arrange
        await ClearDatabaseAsync();

        List<string> dishTags = new();
        int numberOfTags = Faker.Random.Int(1, 4);
        while (numberOfTags-- > 0)
        {
            dishTags.Add(Faker.Name.FullName());
        }

        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        dishTags.Add(dish.Tags[0].Value);
        context.ChangeTracker.Clear();

        // Act
        Result result = await Sender.Send(new AddTagsCommand(dish.Id, dishTags));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.Tags));
        dbDish.Tags.Count.Should().Be(dish.Tags.Count + dishTags.Count - 1);

        foreach (string tag in dishTags)
        {
            dbDish.Tags.Any(t => t.Value == tag).Should().BeTrue();
        }
    }


    [Fact]
    public async Task Should_NotAddTags_NoTagsProvided()
    {
        // Arrange
        await ClearDatabaseAsync();
        List<string> dishTags = new();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new AddTagsCommand(dish.Id, dishTags));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.InvalidTags);
        dbDish.Should().BeEquivalentTo(dish);
    }

    [Fact]
    public async Task Should_NotAddTags_DishNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var dishId = Guid.NewGuid();
        List<string> dishTag = [Faker.Name.FullName()];

        // Act
        Result result = await Sender.Send(new AddTagsCommand(dishId, dishTag));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }
}
