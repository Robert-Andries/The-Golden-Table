using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;
using GoldenTable.Modules.Catalog.Application.Dishes.DeleteDish;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.ValueObjects;
using GoldenTable.Modules.Orders.Tests.IntegrationTests.Abstractions;
using CatalogDishSize = GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject.DishSize;

namespace GoldenTable.Modules.Orders.Tests.IntegrationTests.Dishes;

/// <summary>
///     The whole path between the modules: a Catalog command, the Catalog outbox, the bus,
///     the Orders inbox and the Orders handler
/// </summary>
public sealed class CatalogToOrdersSync(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_CopyDish_WhenCatalogCreatesIt()
    {
        // Act
        Result<Guid> result = await SendAsync(NewCatalogDish(18.9m, [new CatalogDishSize("Family", 9, 1200)]));

        // Assert
        result.IsSuccess.Should().BeTrue();
        Dish copy = await WaitForDishCopyAsync(result.Value);
        copy.BasePrice.Should().Be(Money.Create(18.9m, "EUR").Value);
        copy.Sizes.Should().BeEquivalentTo([new DishSize("Family", 9, 1200)]);
    }

    [Fact]
    public async Task Should_DeleteDishCopy_WhenCatalogDeletesIt()
    {
        // Arrange
        Result<Guid> created = await SendAsync(NewCatalogDish(10m, []));
        await WaitForDishCopyAsync(created.Value);

        // Act
        Result result = await SendAsync(new DeleteDishCommand(created.Value));

        // Assert
        result.IsSuccess.Should().BeTrue();
        await WaitUntilAsync(async () => await FindDishInDbAsync(created.Value) is null,
            $"the copy of dish '{created.Value}' to be deleted");
    }

    private static CreateDishCommand NewCatalogDish(decimal price, List<CatalogDishSize> sizes)
    {
        return new CreateDishCommand(
            $"Dish {Guid.NewGuid():N}",
            "Made for the sync test",
            price,
            "EUR",
            sizes,
            500,
            10,
            150,
            20,
            30,
            2,
            "Pizza",
            []);
    }

    private async Task<Dish> WaitForDishCopyAsync(Guid dishId)
    {
        Dish? copy = null;
        await WaitUntilAsync(async () =>
        {
            copy = await FindDishInDbAsync(dishId);
            return copy is not null;
        }, $"the copy of dish '{dishId}' to be created");

        return copy!;
    }
}
