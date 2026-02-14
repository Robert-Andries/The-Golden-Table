namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

public sealed record Name(string Value)
{
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Value);
    }
};


