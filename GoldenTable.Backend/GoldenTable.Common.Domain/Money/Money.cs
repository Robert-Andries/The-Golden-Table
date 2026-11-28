namespace GoldenTable.Common.Domain.Money;

/// <summary>
///     An immutable value object representing an amount of money in a currency.
///     Operations never change an instance, they return a new one.
/// </summary>
public sealed record Money
{
    private Money()
    {
    }

    /// <summary>
    ///     The amount of money
    /// </summary>
    public decimal Amount { get; private init; }

    /// <summary>
    ///     The currency of that money object
    /// </summary>
    public Currency Currency { get; private init; }

    /// <summary>
    ///     Factory method to create a money object
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
    ///     Factory method to create a money object
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
    ///     Adds the amount of the other money object to this one.
    /// </summary>
    /// <remarks>Neither object is modified: the sum is returned as a new money object</remarks>
    /// <returns>Result holding the sum, or the error that occured</returns>
    public Result<Money> Add(Money other)
    {
        if (Currency != other.Currency)
        {
            return Result.Failure<Money>(MoneyErrors.CurrencyDiffer);
        }

        return Create(Amount + other.Amount, Currency);
    }

    /// <summary>
    ///     Subtracts the amount of the other money object from this one.
    /// </summary>
    /// <remarks>Neither object is modified: the difference is returned as a new money object</remarks>
    /// <returns>Result holding the difference, or the error that occured</returns>
    public Result<Money> Subtract(Money other)
    {
        if (Currency != other.Currency)
        {
            return Result.Failure<Money>(MoneyErrors.CurrencyDiffer);
        }

        if (other.Amount > Amount)
        {
            return Result.Failure<Money>(MoneyErrors.NotEnoughMoneyToSubtract);
        }

        return Create(Amount - other.Amount, Currency);
    }
}
