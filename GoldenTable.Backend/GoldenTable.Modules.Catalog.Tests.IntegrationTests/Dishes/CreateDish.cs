using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Dishes.CreateDish;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public sealed class CreateDish : DishesBaseTest
{
    public CreateDish(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateDish_Successfully()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();

        // Act
        Result<Guid> result = await Sender.Send(new CreateDishCommand(
            dish.Name.Value,
            dish.Description.Value,
            dish.BasePrice.Amount,
            dish.BasePrice.Currency.Code,
            dish.Sizes.ToList(),
            dish.NutritionalInformation.Energy.Kcal,
            dish.NutritionalInformation.GramsOfFat,
            dish.NutritionalInformation.GramsOfCarbohydrates.Total,
            dish.NutritionalInformation.GramsOfCarbohydrates.OfWhichSugar,
            dish.NutritionalInformation.GramsOfProtein,
            dish.NutritionalInformation.GramsOfSalt,
            dish.Category.Name,
            dish.Tags.ToList()));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        context.Dishes.First(x => result.Value == x.Id).Should().BeEquivalentTo(dish, opts =>
            opts.Excluding(d => d.Id).Excluding(d => d.CreatedOnUtc));
    }

    [Fact]
    public async Task Should_NotCreateDish_DishAlreadyExists()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();
        await PutDishInDb(dish);

        // Act
        Result<Guid> result = await Sender.Send(new CreateDishCommand(
            dish.Name.Value,
            dish.Description.Value,
            dish.BasePrice.Amount,
            dish.BasePrice.Currency.Code,
            dish.Sizes.ToList(),
            dish.NutritionalInformation.Energy.Kcal,
            dish.NutritionalInformation.GramsOfFat,
            dish.NutritionalInformation.GramsOfCarbohydrates.Total,
            dish.NutritionalInformation.GramsOfCarbohydrates.OfWhichSugar,
            dish.NutritionalInformation.GramsOfProtein,
            dish.NutritionalInformation.GramsOfSalt,
            dish.Category.Name,
            dish.Tags.ToList()));

        // Assert
        result.Error.Should().Be(DishErrors.DishAlreadyExists);
        context.Dishes.Count().Should().Be(1);
    }

    [Fact]
    public async Task Should_NotCreateDish_DishDomainError()
    {
        // Arrange
        await ClearDatabaseAsync();
        Dish dish = DishBuilder.Build();

        // Act
        Result<Guid> result = await Sender.Send(new CreateDishCommand(
            "",
            dish.Description.Value,
            dish.BasePrice.Amount,
            dish.BasePrice.Currency.Code,
            dish.Sizes.ToList(),
            dish.NutritionalInformation.Energy.Kcal,
            dish.NutritionalInformation.GramsOfFat,
            dish.NutritionalInformation.GramsOfCarbohydrates.Total,
            dish.NutritionalInformation.GramsOfCarbohydrates.OfWhichSugar,
            dish.NutritionalInformation.GramsOfProtein,
            dish.NutritionalInformation.GramsOfSalt,
            dish.Category.Name,
            dish.Tags.ToList()));

        // Assert
        result.Error.Should().Be(DishErrors.InvalidName);
        context.Dishes.Count().Should().Be(0);
    }
}
