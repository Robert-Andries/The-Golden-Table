using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

public static class MoneyErrors
{
    public static Error InvalidAmount { get; } = 
        new("Money.InvalidAmount", "The provided amount is invalid.", ErrorType.Validation);
    
    public static Error InvalidCurrency { get; } = 
        new("Money.InvalidAmount", "The provided currency is invalid.", ErrorType.Validation);

    public static Error CurrencyDiffer { get; } = 
        new("Money.CurrencyDiffer", "The provided currency is different then the other currency.",
            ErrorType.Validation);
}
