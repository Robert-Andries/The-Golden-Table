using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes;
using GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByName;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class GetDishesByName : DishesBaseTest
{
    public GetDishesByName(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_GetDishesByName_FullName_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        DishResponse expectedResponse = new(dish);

        var query = new GetDishesByNameQuery(dish.Name);

        // Act
        Result<List<DishResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value.Single().Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Should_GetDishesByName_PartOfName_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish1 = DishBuilder.WithName(new("ATESTTEST")).Build();
        Dish dish2 = DishBuilder.WithName(new("ATEST1234")).Build();
        Dish dish3 = DishBuilder.Build();
        await PutDishInDb(dish1);
        await PutDishInDb(dish2);
        await PutDishInDb(dish3);
        List<DishResponse> expectedResponse = [new(dish1), new(dish2)];
        string partOfNameStr = "ATEST";
        Name partialName = new(partOfNameStr);
        
        // Act
        Result<List<DishResponse>> result = await Sender.Send(new GetDishesByNameQuery(partialName));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();
        result.Value.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Should_GetDishesByName_NoDish()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        Name nonExistentName = new (dish.Name.Value + "NonExistentSuffix");
        var query = new GetDishesByNameQuery(nonExistentName);

        // Act
        Result<List<DishResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
