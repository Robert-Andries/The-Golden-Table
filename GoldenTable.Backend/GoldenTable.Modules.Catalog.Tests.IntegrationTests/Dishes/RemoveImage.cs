using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.AddImage;
using GoldenTable.Modules.Catalog.Application.Dishes.RemoveImage;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class RemoveImage : DishesBaseTest
{
    public RemoveImage(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_RemoveImage_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        await Sender.Send(new AddImageCommand(dish.Id, image.Id));
        context.ChangeTracker.Clear();

        // Act
        Result result = await Sender.Send(new RemoveImageCommand(dish.Id, image.Id));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.Images));
        dbDish.Images.Count.Should().Be(dish.Images.Count - 1);
        foreach (Image uniqueImage in dish.Images.Where(i => i.Id != image.Id).ToList())
        {
            dbDish.Images.Contains(uniqueImage).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Should_NotRemoveImage_DishNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var dishId = Guid.NewGuid();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);

        // Act
        Result result = await Sender.Send(new RemoveImageCommand(dishId, image.Id));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }

    [Fact]
    public async Task Should_NotRemoveImage_ImageNotFound()
    {
        //Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        var imageId = Guid.NewGuid();

        // Act
        Result result = await Sender.Send(new RemoveImageCommand(dish.Id, imageId));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.ImageNotPresent);
        dbDish.Should().BeEquivalentTo(dish);
    }
}
