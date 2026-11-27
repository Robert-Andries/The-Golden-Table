using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Moneys;

public class Subtract : MoneyBaseTest
{
    [Fact]
    public void Should_SubtractMoney_Successfully()
    {
        // Arrange
        Money original = MoneyFaker.Generate();
        Money sut = original.DeepClone();
        Money sutCopy = sut.DeepClone();

        // Act
        Result<Money> result = sut.Subtract(original);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Currency.Should().Be(original.Currency);
        result.Value.Amount.Should().Be(0);
        result.Value.Should().NotBeSameAs(sut);
        sut.Should().BeEquivalentTo(sutCopy);
    }

    [Fact]
    public void Should_SubtractMoney_FromItself()
    {
        // Arrange
        Money sut = MoneyFaker.Generate();
        Money copy = sut.DeepClone();

        // Act
        Result<Money> result = sut.Subtract(sut);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(0);
        sut.Should().BeEquivalentTo(copy);
    }

    [Fact]
    public void Should_NotSubtractMoney_DiffrentCurrency()
    {
        // Arrange
        Money original = MoneyFaker.Generate();
        Currency differentCurrency = new[] { Currency.Usd, Currency.Ron, Currency.Eur }.First(c => c != original.Currency);
        decimal validAmount = original.Amount + 10m;
        Money sut = Money.Create(validAmount, differentCurrency).Value;
        Money copy = sut.DeepClone();

        // Act
        Result<Money> result = sut.Subtract(original);

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
        Money sut = original.DeepClone();
        Money larger = Money.Create(original.Amount + 1m, original.Currency).Value;

        // Act
        Result<Money> result = sut.Subtract(larger);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.NotEnoughMoneyToSubtract);
        sut.Should().BeEquivalentTo(original);
    }
}
