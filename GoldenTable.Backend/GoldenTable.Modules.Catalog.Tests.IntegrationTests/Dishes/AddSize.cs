using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.AddSize;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class AddSize : DishesBaseTest
{
    public AddSize(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_AddSize_Succesfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        DishSize newSize = new(Faker.Name.FullName(), Faker.Random.Float(-10F, 10F), Faker.Random.Float(10F, 100F));
        // Act
        Result result =
            await Sender.Send(new AddSizeCommand(dish.Id, newSize.Name, newSize.PriceAdded, newSize.Weight));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, options => options.Excluding(size => size.Sizes));
        dbDish.Sizes.Contains(newSize).Should().BeTrue();
    }

    [Fact]
    public async Task Should_NotAddSize_SizeAlreadyPresent()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishSize newSize = new(Faker.Name.FullName(), Faker.Random.Float(-10F, 10F), Faker.Random.Float(10F, 100F));
        Dish dish = DishBuilder.WithSizes([newSize]).Build();
        await PutDishInDb(dish);
        // Act
        Result result =
            await Sender.Send(new AddSizeCommand(dish.Id, newSize.Name, newSize.PriceAdded, newSize.Weight));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.SizeAlreadyPresent);
        dbDish.Should().BeEquivalentTo(dish);
    }
}
