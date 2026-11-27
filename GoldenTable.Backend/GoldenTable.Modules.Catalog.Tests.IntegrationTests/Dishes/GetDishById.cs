using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishById;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class GetDishById : DishesBaseTest
{
    public GetDishById(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnDishResponse_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        Guid id = dish.Id;
        DishResponse response = new(dish);

        // Act
        Result<DishResponse> result = await Sender.Send(new GetDishByIdQuery(id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Should_ReturnDishResponse_WhenDishIsCached()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutTagsInDb(dish.Tags);
        // Creating the dish also stores it in the cache
        Result<Guid> createResult = await Sender.Send(ToCreateDishCommand(dish));

        // Act
        Result<DishResponse> result = await Sender.Send(new GetDishByIdQuery(createResult.Value));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(dish.Name.Value);
        result.Value.BasePriceAmount.Should().Be(dish.BasePrice.Amount);
        result.Value.BasePriceCurrency.Should().Be(dish.BasePrice.Currency.Code);
    }

    [Fact]
    public async Task Should_ReturnError_DishNotFound()
    {
        // Arrange
        var dishId = Guid.NewGuid();

        // Act
        Result<DishResponse> result = await Sender.Send(new GetDishByIdQuery(dishId));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }
}
