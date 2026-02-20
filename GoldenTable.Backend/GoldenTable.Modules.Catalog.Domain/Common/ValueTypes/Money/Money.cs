using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

public sealed record Money
{
    private Money()
    { }

    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public static Result<Money> Create(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            return Result.Failure<Money>(MoneyErrors.InvalidAmount);
        }
        
        var money = new Money
        {
            Amount = amount,
            Currency = currency
        };
        
        return money;
    }
    
    public static Result<Money> Create(decimal amount, string currency)
    {
        Result<Currency> currencyResult = Currency.FromCode(currency);
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Money>(currencyResult.Error);
        }
        return Create(amount, currencyResult.Value);
    }

    public Result Add(Money other)
    {
        if (Currency != other.Currency)
        {
            return MoneyErrors.CurrencyDiffer;
        }
        Amount += other.Amount;
        return Result.Success();
    }
    
    public Result Subtract(Money other)
    {
        if (Currency != other.Currency)
        {
            return MoneyErrors.CurrencyDiffer;
        }
        Amount -= other.Amount;
        return Result.Success();
    }
}
