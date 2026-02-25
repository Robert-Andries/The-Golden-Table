using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;

/// <summary>
/// Record used to storing data related to money's currency
/// </summary>
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

    /// <summary>
    /// Gets a Currency object 
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static Result<Currency> FromCode(string code)
    {
        return code switch
        {
            "EUR" => EUR,
            "RON" => RON,
            "USD" => USD,
            _ => Result.Failure<Currency>(MoneyErrors.InvalidCurrency),
        };
    }
    /// <summary>
    /// Gets the currency's code
    /// E.g. EUR/USD
    /// </summary>
    public string Code { get; private set; }
    /// <summary>
    /// Gets the currency's name
    /// E.g. US Dollars, Euro
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets the currency's symbol
    /// E.g. $
    /// </summary>
    public string Symbol { get; }

    public static Currency EUR { get; } = new("EUR", "EURO", "€");
    public static Currency RON { get; } = new("RON", "RON");
    public static Currency USD { get; } = new("USD", "American Dollar", "$");
}
