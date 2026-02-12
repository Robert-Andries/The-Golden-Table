namespace GoldenTable.Modules.Catalog.Domain.Common.Money;

public record Currency
{
    private Currency(string code, string name, string? symbol = null)
    {
        Code = code;
        Name = name;
        if (string.IsNullOrEmpty(symbol))
        {
            Symbol = Code;
        }
    }

    public string Code { get; private set; }
    public string Name { get; set; }

    public string Symbol { get; }

    public static Currency EUR { get; } = new("EUR", "EURO", "€");
    public static Currency RON { get; } = new("RON", "RON");
    public static Currency USD { get; } = new("USD", "American Dollar", "$");
}
