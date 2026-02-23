using FluentAssertions;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

namespace GoldenTable.Modules.Catalog.Tests.Moneys;

public sealed class CreateMoney : BaseTest
{
    [Fact]
    public void Should_CreateMoneyConstructor1_Successfully()
    {
        // Arrange
        decimal amount = Faker.Random.Decimal(0m, 100m);
        Currency currency = Currency.USD;
        
        // Act
        Result<Money> result = Money.Create(amount, currency);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(amount);
        result.Value.Currency.Should().Be(currency);
    }
    
    [Fact]
    public void Should_CreateMoneyConstructor2_Successfully()
    {
        // Arrange
        decimal amount = Faker.Random.Decimal(0m, 100m);
        string currency = "USD";
        
        // Act
        Result<Money> result = Money.Create(amount, currency);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(amount);
        result.Value.Currency.Should().Be(Currency.USD);
    }
    [Fact]
    public void Should_NotCreateMoneyConstructor2_InvalidCurrency()
    {
        // Arrange
        decimal amount = Faker.Random.Decimal(0m, 100m);
        string currency = "SomeInvalidCurrencyCode";
        
        // Act
        Result<Money> result = Money.Create(amount, currency);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.InvalidCurrency);
    }
    
    [Fact]
    public void Should_NotCreateMoneyConstructor1_InvalidAmount()
    {
        // Arrange
        decimal amount = Faker.Random.Decimal(-100M, -1M);
        Currency currency = Currency.USD;
        
        // Act
        Result<Money> result = Money.Create(amount, currency);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.InvalidAmount);
    }
    
    [Fact]
    public void Should_NotCreateMoneyConstructor2_InvalidAmount()
    {
        // Arrange
        decimal amount = Faker.Random.Decimal(-100M, -1M);
        string currency = "USD";
        
        // Act
        Result<Money> result = Money.Create(amount, currency);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.InvalidAmount);
    }
}
