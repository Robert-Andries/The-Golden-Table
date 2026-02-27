using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.AddTags;

public sealed record AddTagsCommand(Guid DishId, List<string> Tags) : ICommand;
