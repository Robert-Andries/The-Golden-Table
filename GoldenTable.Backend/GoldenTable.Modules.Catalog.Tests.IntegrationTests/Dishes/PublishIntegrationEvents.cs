using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.DeleteDish;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using GoldenTable.Modules.Catalog.IntegrationEvents;
using GoldenTable.Modules.Catalog.IntegrationEvents.Models;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class PublishIntegrationEvents : DishesBaseTest, IAsyncLifetime
{
    private readonly PublishedMessages _publishedMessages = new();
    private readonly ConnectHandle _publishObserverHandle;

    public PublishIntegrationEvents(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _publishObserverHandle = factory.Services.GetRequiredService<IBus>().ConnectPublishObserver(_publishedMessages);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _publishObserverHandle.Disconnect();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Should_PublishDishCreatedIntegrationEvent()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.WithSizes([new DishSize("Small", 0, 200)]).Build();
        await PutTagsInDb(dish.Tags);

        // Act
        Result<Guid> result = await Sender.Send(ToCreateDishCommand(dish));

        // Assert
        result.IsSuccess.Should().BeTrue();
        DishCreatedIntegrationEvent integrationEvent =
            await _publishedMessages.WaitForAsync<DishCreatedIntegrationEvent>(e => e.DishId == result.Value);
        integrationEvent.Name.Should().Be(dish.Name.Value);
        integrationEvent.BasePriceAmount.Should().Be(dish.BasePrice.Amount);
        integrationEvent.BasePriceCurrency.Should().Be(dish.BasePrice.Currency.Code);
        integrationEvent.Sizes.Should().BeEquivalentTo([new DishSizeModel("Small", 0, 200)]);
    }

    [Fact]
    public async Task Should_PublishDishUpdatedNameIntegrationEvent()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        string newName = Faker.Name.FullName() + Guid.NewGuid().ToString("N");

        // Act
        Result result = await Sender.Send(CreateEditDishCommand(dish, name: newName));

        // Assert
        result.IsSuccess.Should().BeTrue();
        DishUpdatedNameIntegrationEvent integrationEvent =
            await _publishedMessages.WaitForAsync<DishUpdatedNameIntegrationEvent>(e => e.DishId == dish.Id);
        integrationEvent.NewName.Should().Be(newName);
    }

    [Fact]
    public async Task Should_PublishDishUpdatedPriceIntegrationEvent()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);
        decimal newPrice = dish.BasePrice.Amount + 1;

        // Act
        Result result = await Sender.Send(CreateEditDishCommand(dish, basePriceAmount: newPrice));

        // Assert
        result.IsSuccess.Should().BeTrue();
        DishUpdatedPriceIntegrationEvent integrationEvent =
            await _publishedMessages.WaitForAsync<DishUpdatedPriceIntegrationEvent>(e => e.DishId == dish.Id);
        integrationEvent.BasePriceAmount.Should().Be(newPrice);
        integrationEvent.BasePriceCurrency.Should().Be(dish.BasePrice.Currency.Code);
    }

    [Fact]
    public async Task Should_PublishDishUpdatedSizesIntegrationEvent()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.WithSizes([new DishSize("Small", 0, 200)]).Build();
        await PutDishInDb(dish);
        List<DishSize> sizes = [new("Medium", 2, 300), new("Large", 5, 400)];

        // Act
        Result result = await Sender.Send(CreateEditDishCommand(dish, sizes: sizes));

        // Assert
        result.IsSuccess.Should().BeTrue();
        DishUpdatedSizesIntegrationEvent integrationEvent =
            await _publishedMessages.WaitForAsync<DishUpdatedSizesIntegrationEvent>(e => e.DishId == dish.Id);
        integrationEvent.Sizes.Should().BeEquivalentTo(sizes.Select(s => new DishSizeModel(s.Name, s.PriceAdded,
            s.Weight)));
    }

    [Fact]
    public async Task Should_PublishDishDeletedIntegrationEvent()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result result = await Sender.Send(new DeleteDishCommand(dish.Id));

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _publishedMessages.WaitForAsync<DishDeletedIntegrationEvent>(e => e.DishId == dish.Id);
    }
}
