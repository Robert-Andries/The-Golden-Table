using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

/// <summary>
/// A class representing money objects
/// </summary>
public sealed record Money
{
    private Money()
    { }

    /// <summary>
    /// The amount of money
    /// </summary>
    public decimal Amount { get; private set; }
    /// <summary>
    /// The currency of that money object
    /// </summary>
    public Currency Currency { get; private set; }

    /// <summary>
    /// Factory method to create a money object
    /// </summary>
    /// <param name="amount">The initial amount</param>
    /// <param name="currency">Currency of that amount</param>
    /// <returns>Result object indicating the success or failure and respective code</returns>
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
    
    /// <summary>
    /// Factory method to create a money object
    /// </summary>
    /// <param name="amount">The initial amount</param>
    /// <param name="currencyCode">Currency code of that amount (e.g. USD)</param>
    /// <returns>Result object indicating the success or failure and respective code</returns>
    public static Result<Money> Create(decimal amount, string currencyCode)
    {
        Result<Currency> currencyResult = Currency.FromCode(currencyCode);
        if (currencyResult.IsFailure)
        {
            return Result.Failure<Money>(currencyResult.Error);
        }
        return Create(amount, currencyResult.Value);
    }

    /// <summary>
    /// Method to add to the current money object the amount of the other money object.
    /// </summary>
    /// <remarks>Adding to the current object does not affect the "other" object passed as argument</remarks>
    /// <returns>Result object indicating the success or failure and respective code</returns>
    public Result Add(Money other)
    {
        if (ReferenceEquals(this, other))
        {
            return MoneyErrors.SameInstance;
        }
        if (Currency != other.Currency)
        {
            return MoneyErrors.CurrencyDiffer;
        }
        Amount += other.Amount;
        return Result.Success();
    }
    
    /// <summary>
    /// Method to subtract from current money object the amount of the other money object.
    /// </summary>
    /// <remarks>Subtracting from the current object does not affect the "other" object passed as argument</remarks>
    /// <returns>Result object indicating the success or failure and respective code</returns>
    public Result Subtract(Money other)
    {
        if (ReferenceEquals(this, other))
        {
            return MoneyErrors.SameInstance;
        }
        if (Currency != other.Currency)
        {
            return MoneyErrors.CurrencyDiffer;
        }
        if (other.Amount > Amount)
        {
            return MoneyErrors.NotEnoughMoneyToSubtract;
        }
        Amount -= other.Amount;
        return Result.Success();
    }
}
