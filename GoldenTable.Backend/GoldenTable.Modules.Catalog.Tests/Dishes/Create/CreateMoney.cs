using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

namespace GoldenTable.Modules.Catalog.Tests.Dishes.Create;

public class CreateMoney : BaseTest
{
    private readonly decimal _validAmount;
    private readonly Currency _validCurrency;

    private readonly string _validCurrencyCode;

    public CreateMoney()
    {
        _validCurrencyCode = "USD";
        _validCurrency = Currency.USD;
        _validAmount = Faker.Random.Decimal(0M, 1000M);
    }

    [Fact]
    public void Should_CreateMoney_Successfully()
    {
        // Act
        Result<Money> moneyResultPrimitiveConstructor = Money.Create(_validAmount, _validCurrencyCode);
        Result<Money> moneyResultSecondConstructor = Money.Create(_validAmount, _validCurrency);

        // Assert
        moneyResultPrimitiveConstructor.IsSuccess.Should().BeTrue();
        moneyResultSecondConstructor.IsSuccess.Should().BeTrue();
        moneyResultPrimitiveConstructor.Value.Amount.Should().Be(_validAmount);
        moneyResultSecondConstructor.Value.Amount.Should().Be(_validAmount);
        moneyResultPrimitiveConstructor.Value.Currency.Should().Be(_validCurrency);
        moneyResultSecondConstructor.Value.Currency.Should().Be(_validCurrency);
    }

    [Fact]
    public void Should_CreateMoneyWithInvalidCurrency_Failure()
    {
        // Arrange
        string invalidCurrencyCode = "WTF";

        // Act
        Result<Money> moneyResult = Money.Create(_validAmount, invalidCurrencyCode);

        // Assert
        moneyResult.IsFailure.Should().BeTrue();
    }
}
