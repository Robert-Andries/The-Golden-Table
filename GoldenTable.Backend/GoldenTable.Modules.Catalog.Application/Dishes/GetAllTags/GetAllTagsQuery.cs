using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetAllTags;

public sealed record GetAllTagsQuery : IQuery<List<TagResponse>>;
