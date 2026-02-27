using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.AddImage;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class AddImage : DishesBaseTest
{
    public AddImage(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_AddImage_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new AddImageCommand(dish.Id, image.Id));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, options => options.Excluding(d => d.Images));
        dbDish.Images.Should().HaveCount(1);
        dbDish.Images.Any(i => i.Id == image.Id).Should().BeTrue();
    }

    [Fact]
    public async Task Should_NotAddImage_DishNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var dishId = Guid.NewGuid();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);

        // Act
        Result result = await Sender.Send(new AddImageCommand(dishId, image.Id));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }

    [Fact]
    public async Task Should_NotAddImage_ImageNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        var imageId = Guid.NewGuid();

        // Act
        Result result = await Sender.Send(new AddImageCommand(dish.Id, imageId));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(ImageErrors.NotFound);
        dbDish.Should().BeEquivalentTo(dish);
    }

    [Fact]
    public async Task Should_NotAddImage_ImageAlreadyExistsInDish()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        Image image = ImageBuilder.Build();
        await PutImageInDb(image);
        await PutDishInDb(dish);
        await Sender.Send(new AddImageCommand(dish.Id, image.Id));

        // Act
        Result result = await Sender.Send(new AddImageCommand(dish.Id, image.Id));

        // Assert
        result.Error.Should().Be(DishErrors.ImageAlreadyPresent);
        dish.Should().BeEquivalentTo(await GetDishFromDb(dish.Id));
    }
}
