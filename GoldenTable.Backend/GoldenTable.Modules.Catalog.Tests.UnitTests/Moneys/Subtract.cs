using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Moneys;

public class Subtract : MoneyBaseTest
{
    [Fact]
    public void Should_SubtractMoney_Successfully()
    {
        // Arrange
        Money original = MoneyFaker.Generate();
        Money sut = original.DeepClone();

        // Act
        Result result = sut.Subtract(original);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.Should().BeEquivalentTo(original, options => options.Excluding(m => m.Amount));
        sut.Amount.Should().Be(0);
    }

    [Fact]
    public void Should_NotSubtractMoney_SameInstance()
    {
        // Arrange
        Money original = MoneyFaker.Generate();
        Money sut = original.DeepClone();

        // Act
        Result result = sut.Subtract(sut);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.SameInstance);
        sut.Should().BeEquivalentTo(original);
    }

    [Fact]
    public void Should_NotSubtractMoney_DiffrentCurrency()
    {
        // Arrange
        Money original = MoneyFaker.Generate();
        Currency differentCurrency = Currency.All.First(c => c != original.Currency);
        decimal validAmount = original.Amount + 10m;
        Money sut = Money.Create(validAmount, differentCurrency).Value;
        Money copy = sut.DeepClone();

        // Act
        Result result = sut.Subtract(original);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.CurrencyDiffer);
        sut.Should().BeEquivalentTo(copy);
    }

    [Fact]
    public void Should_NotSubtractMoney_NotEnoughMoneyToSubtract()
    {
        // Arrange
        Money original = MoneyFaker.Generate();
        Money copy = original.DeepClone();
        Money sut = original.DeepClone();
        copy.Add(original);

        // Act
        Result result = sut.Subtract(copy);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.NotEnoughMoneyToSubtract);
        sut.Should().BeEquivalentTo(original);
    }
}
