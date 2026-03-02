using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByTags;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class GetDishesByTags : DishesBaseTest
{
    public GetDishesByTags(IntegrationTestWebAppFactory factory) : base(factory)
    { }

    [Fact]
    public async Task Should_GetDishesByOneTag_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishTag commonTag = DishTag.Create("CommonTag").Value;
        List<DishResponse> expectedResponse = new();
        int numberOfDishes = Faker.Random.Int(3, 8);
        while (numberOfDishes-- > 0)
        {
            Dish dish = DishBuilder.WithTags([commonTag]).Build();
            await PutDishInDb(dish);
            expectedResponse.Add(new(dish));
        }

        // Act
        Result<List<DishResponse>> result = await Sender.Send(new GetDishesByTagsQuery([commonTag]));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(expectedResponse.Count);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task Should_GetDishesByTags_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        List<DishTag> commonTags = [DishTag.Create("CommonTag").Value, DishTag.Create("CommonTag2").Value];
        List<DishResponse> expectedResponse = new();
        int numberOfDishes = Faker.Random.Int(3, 8);
        while (numberOfDishes-- > 0)
        {
            Dish dish = DishBuilder.WithTags(commonTags).Build();
            await PutDishInDb(dish);
            expectedResponse.Add(new(dish));
        }

        // Act
        Result<List<DishResponse>> result = await Sender.Send(new GetDishesByTagsQuery(commonTags));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(expectedResponse.Count);
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task Should_GetNoDishesBySomeUniqueTag_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        DishTag uniqueTag = DishTag.Create("UniqueTagThatNoDishHas_osduhfisodhfsd").Value;
        int numberOfDishes = Faker.Random.Int(3, 8);
        while (numberOfDishes-- > 0)
        {
            Dish dish = DishBuilder.Build();
            await PutDishInDb(dish);
        }

        // Act
        Result<List<DishResponse>> result = await Sender.Send(new GetDishesByTagsQuery([uniqueTag]));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }
}
