namespace GoldenTable.Modules.Catalog.Domain.Common.Money;

internal sealed class Money
{
    private Money(decimal value, Currency currency)
    {
        Value = value;
        Currency = currency;
    }

    public decimal Value { get; private set; }
    public Currency Currency { get; private set; }

    public Money Create(decimal value, Currency currency)
    {
        return new(value, currency);
    }
}
