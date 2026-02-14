using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.Enums;

namespace GoldenTable.Modules.Catalog.Application.Dishes.AddTags;

public sealed record AddTagsCommand(Guid DishId, List<DishTag> Tags) : ICommand;
