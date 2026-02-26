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
        Money sut = MoneyFaker.Generate();
        for (int i = 0; i < 100 && sut.Currency == original.Currency && sut.Amount >= original.Amount; i++)
        {
            if (i == 99)
            {
                throw new Exception("Cannot make 2 different currencies");
            }
            sut = MoneyFaker.Generate();
        }
        Money copy = sut.DeepClone();

        // Act
        Result result = sut.Add(original);

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
