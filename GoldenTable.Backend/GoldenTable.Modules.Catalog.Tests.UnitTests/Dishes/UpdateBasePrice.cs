using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.Events;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Dishes;

public sealed class UpdateBasePrice : DishBaseTest
{
    [Fact]
    public void Should_UpdateBasePrice_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        decimal validAmount = Faker.Random.Decimal(1M, 100M);
        Money money = Money.Create(validAmount, Currency.RON).Value;

        // Act
        Result result = sut.UpdateBasePrice(money, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options =>
        {
            return options.Excluding(d => d.BasePrice)
                .Excluding(d => d.ModifiedOnUtc)
                .Excluding(d => d.DomainEvents);
        });
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        sut.BasePrice.Should().Be(money);
        AssertDomainEventWasPublished<DishUpdatedBasePriceDomainEvent>(sut);
    }

    [Fact]
    public void Should_NotUpdateBasePrice_OldPriceSameAsNewOne()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Money money = Money.Create(sut.BasePrice.Amount, sut.BasePrice.Currency).Value;

        // Act
        Result result = sut.UpdateBasePrice(money, SometimeUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        sut.Should().BeEquivalentTo(original);
    }
}
