using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;

namespace GoldenTable.Modules.Catalog.Application.Dishes;

public sealed class TagResponse
{
    public TagResponse(DishTag tag)
    {
        Id = tag.Id;
        Value = tag.Value;
    }

    public Guid Id { get; set; }
    public string Value { get; set; }
}
