using FluentAssertions;
using Force.DeepCloner;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;

namespace GoldenTable.Modules.Catalog.Tests.UnitTests.Moneys;

public sealed class Add : MoneyBaseTest
{
    [Fact]
    public void Should_AddMoney_Successfully()
    {
        // Arrange
        Money original = MoneyFaker.Generate();
        Money sut = original.DeepClone();
        Money sutCopy = sut.DeepClone();

        // Act
        Result<Money> result = sut.Add(original);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Currency.Should().Be(original.Currency);
        result.Value.Amount.Should().Be(original.Amount * 2);
        result.Value.Should().NotBeSameAs(sut);
        sut.Should().BeEquivalentTo(sutCopy);
        original.Should().BeEquivalentTo(sutCopy);
    }

    [Fact]
    public void Should_AddMoney_ToItself()
    {
        // Arrange
        Money sut = MoneyFaker.Generate();
        Money copy = sut.DeepClone();

        // Act
        Result<Money> result = sut.Add(sut);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(copy.Amount * 2);
        sut.Should().BeEquivalentTo(copy);
    }

    [Fact]
    public void Should_NotAddMoney_DiffrentCurrency()
    {
        // Arrange
        Money original = MoneyFaker.Generate();
        Currency differentCurrency = new[] { Currency.Usd, Currency.Ron, Currency.Eur }.First(c => c != original.Currency);
        Money sut = Money.Create(original.Amount, differentCurrency).Value;
        Money copy = sut.DeepClone();

        // Act
        Result<Money> result = sut.Add(original);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.CurrencyDiffer);
        sut.Should().BeEquivalentTo(copy);
    }
}
