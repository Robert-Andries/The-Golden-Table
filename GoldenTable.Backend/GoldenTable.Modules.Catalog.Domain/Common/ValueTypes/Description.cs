namespace GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

public record Description(string Value)
{
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Value);
    }
};
