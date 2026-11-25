using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Orders.Domain.Dishes;
using GoldenTable.Modules.Orders.Domain.Dishes.Events;

namespace GoldenTable.Modules.Orders.Tests.UnitTests.Dishes;

public sealed class UpdateBasePrice : DishBaseTest
{
    [Fact]
    public void Should_UpdateBasePrice_Successfully()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        decimal validAmount = Faker.Random.Decimal(1M, 100M);
        Money money = Money.Create(validAmount, Currency.Ron).Value;

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
        sut.BasePrice.Should().Be(money);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        DishUpdatedPriceDomainEvent domainEvent = AssertDomainEventWasPublished<DishUpdatedPriceDomainEvent>(sut);
        domainEvent.DishId.Should().Be(sut.Id);
        domainEvent.NewBasePriceAmount.Should().Be(money.Amount);
        domainEvent.NewBasePriceCurrency.Should().Be(money.Currency.Code);
    }

    [Fact]
    public void Should_UpdateBasePrice_SameAmountDifferentCurrency()
    {
        // Arrange
        Dish original = DishFaker.Generate();
        Dish sut = original.DeepClone();
        Money money = Money.Create(sut.BasePrice.Amount, Currency.Eur).Value;

        // Act
        Result result = sut.UpdateBasePrice(money, SometimeUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.BasePrice.Should().Be(money);
        sut.ModifiedOnUtc.Should().Be(SometimeUtc);
        AssertDomainEventWasPublished<DishUpdatedPriceDomainEvent>(sut)
            .NewBasePriceCurrency.Should().Be(Currency.Eur.Code);
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
        result.Error.Should().Be(DishErrors.SamePrice);
        sut.Should().BeEquivalentTo(original);
        sut.DomainEvents.Should().BeEmpty();
    }
}
