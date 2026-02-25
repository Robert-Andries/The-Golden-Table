namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

/// <summary>
/// Value object holding the necesarry data for a description
/// </summary>
/// <param name="Value">The description text</param>
public record Description(string Value)
{
    /// <returns>True - the description is valid, false otherwise</returns>
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Value);
    }
};
