using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes;
using GoldenTable.Modules.Catalog.Application.Dishes.GetAllDishes;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class GetAllDishes : DishesBaseTest
{
    public GetAllDishes(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_GetAllDishes_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        List<Dish> dishes = new();
        int numberOfdishes = Faker.Random.Int(3, 10);
        while (numberOfdishes-- > 0)
        {
            Dish dish = DishBuilder.Build();
            await PutDishInDb(dish);
            dishes.Add(dish);
            context.ChangeTracker.Clear();
        }

        var expectedResponse = dishes.Select(d => new DishResponse(d)).ToList();

        // Act
        Result<List<DishResponse>> response = await Sender.Send(new GetAllDishesQuery());

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Value.Count.Should().Be(dishes.Count);
        foreach (DishResponse dishResponse in response.Value)
        {
            expectedResponse.Any(d => d.Name == dishResponse.Name).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Should_GetNoDishes_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        Result<List<DishResponse>> response = await Sender.Send(new GetAllDishesQuery());

        // Assert
        response.IsSuccess.Should().BeTrue();
        response.Value.Count.Should().Be(0);
    }
}
