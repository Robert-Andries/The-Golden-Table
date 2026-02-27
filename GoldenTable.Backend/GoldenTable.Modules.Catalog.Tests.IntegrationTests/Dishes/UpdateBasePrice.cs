using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.UpdateBasePrice;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class UpdateBasePrice : DishesBaseTest
{
    public UpdateBasePrice(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_UpdateBasePrice_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        decimal newAmount = Faker.Random.Decimal(1M, 100M);
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new UpdateBasePriceCommand(dish.Id, newAmount));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.IsSuccess.Should().BeTrue();
        dbDish.Should().BeEquivalentTo(dish, opts => opts.Excluding(d => d.BasePrice));
        dbDish.BasePrice.Amount.Should().Be(newAmount);
        dbDish.BasePrice.Currency.Should().Be(dish.BasePrice.Currency);
    }

    [Fact]
    public async Task Should_NotUpdateBasePrice_DishNotFound()
    {
        // Arrange
        await ClearDatabaseAsync();
        var dishId = Guid.NewGuid();
        decimal newAmount = Faker.Random.Decimal(1M, 100M);

        // Act
        Result result = await Sender.Send(new UpdateBasePriceCommand(dishId, newAmount));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }

    [Fact]
    public async Task Should_NotUpdateBasePrice_InvalidAmount()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        decimal newAmount = Faker.Random.Decimal(-100M, -1M);

        // Act
        Result result = await Sender.Send(new UpdateBasePriceCommand(dish.Id, newAmount));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(MoneyErrors.InvalidAmount);
        dbDish.Should().BeEquivalentTo(dish);
    }

    [Fact]
    public async Task Should_NotUpdateBasePrice_SamePriceAsBefore()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        decimal newAmount = dish.BasePrice.Amount;

        // Act
        Result result = await Sender.Send(new UpdateBasePriceCommand(dish.Id, newAmount));

        // Assert
        Dish dbDish = await GetDishFromDb(dish.Id);
        result.Error.Should().Be(DishErrors.SamePrice);
        dbDish.Should().BeEquivalentTo(dish);
    }
}
