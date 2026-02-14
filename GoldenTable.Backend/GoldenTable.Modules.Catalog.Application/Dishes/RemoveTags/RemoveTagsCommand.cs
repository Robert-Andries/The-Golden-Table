using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Dishes.Enums;

namespace GoldenTable.Modules.Catalog.Application.Dishes.RemoveTags;

public sealed record RemoveTagsCommand(Guid DishId, List<DishTag> Tags) : ICommand;
