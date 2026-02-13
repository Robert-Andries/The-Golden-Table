using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

public static class MoneyErrors
{
    public static Error InvalidAmount { get; } = 
        new Error("Money.InvalidAmount", "The provided amount is invalid.", ErrorType.Validation);
}
