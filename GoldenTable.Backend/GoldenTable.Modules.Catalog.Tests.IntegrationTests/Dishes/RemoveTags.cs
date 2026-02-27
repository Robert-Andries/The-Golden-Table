using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.RemoveTags;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class RemoveTags : DishesBaseTest
{
    public RemoveTags(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_RemoveTags_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        int numberOfTagsToRemove = dish.Tags.Count - Faker.Random.Int(1, dish.Tags.Count - 1);
        var tags = Faker.PickRandom(dish.Tags, numberOfTagsToRemove).ToList();
        context.ChangeTracker.Clear();

        // Act
        Result result = await Sender.Send(new RemoveTagsCommand(dish.Id, tags));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.Tags).Excluding(d => d.ModifiedOnUtc));
        dbDish.Tags.Count.Should().Be(dish.Tags.Count - tags.Count);
        foreach (DishTag dishTag in dish.Tags.Where(t => !tags.Contains(t)).ToList())
        {
            dbDish.Tags.Any(t => t.Value == dishTag.Value).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Should_NotRemoveTags_TagsEmpty()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        List<DishTag> tags = new();

        // Act
        Result result = await Sender.Send(new RemoveTagsCommand(dish.Id, tags));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.InvalidTags);
        dbDish.Should().BeEquivalentTo(dish);
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenTagsDoesNotExistInDish()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        List<DishTag> tags = [DishTag.Create(Faker.Name.FullName()).Value];

        // Act
        Result result = await Sender.Send(new RemoveTagsCommand(dish.Id, tags));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish);
    }
}
