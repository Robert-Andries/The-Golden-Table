using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

public sealed record Money
{
    private Money()
    { }

    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public Result<Money> Create(decimal amount, Currency currency)
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
}
