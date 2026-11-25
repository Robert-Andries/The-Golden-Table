namespace GoldenTable.Common.Domain.Money;

public static class MoneyErrors
{
    public static Error InvalidAmount { get; } =
        new("Money.InvalidAmount", "The provided amount is invalid.", ErrorType.Validation);

    public static Error InvalidCurrency { get; } =
        new("Money.InvalidCurrency", "The provided currency is invalid.", ErrorType.Validation);

    public static Error CurrencyDiffer { get; } =
        new("Money.CurrencyDiffer", "The provided currency is different then the other currency.",
            ErrorType.Validation);

    public static Error NotEnoughMoneyToSubtract { get; } =
        new("Money.NotEnoughMoneyToSubtract", "The provided amount is higher then the current amount.",
            ErrorType.Validation);
}
