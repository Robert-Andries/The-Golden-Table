using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Infrastructure.Outbox;
using GoldenTable.Modules.Catalog.Application.Dishes.DeleteDish;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class DeleteDish : DishesBaseTest
{
    public DeleteDish(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_DeleteDish_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new DeleteDishCommand(dish.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await context.Dishes.AsNoTracking().AnyAsync(d => d.Id == dish.Id)).Should().BeFalse();
        (await context.Set<OutboxMessage>().AsNoTracking()
                .AnyAsync(m => m.Type == nameof(DishDeletedDomainEvent)))
            .Should().BeTrue();
    }

    [Fact]
    public async Task Should_NotDeleteDish_DishDoesNotExist()
    {
        // Arrange
        await ClearDatabaseAsync();

        // Act
        Result result = await Sender.Send(new DeleteDishCommand(Guid.NewGuid()));

        // Assert
        result.Error.Should().Be(DishErrors.NotFound);
    }
}
