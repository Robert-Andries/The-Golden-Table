namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

/// <summary>
/// Value obect used to hold the data necesarry for a name
/// </summary>
public sealed record Name(string Value)
{
    /// <returns>True - the name is valid; False - otherwise</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Value);
    }
};


