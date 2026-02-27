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
