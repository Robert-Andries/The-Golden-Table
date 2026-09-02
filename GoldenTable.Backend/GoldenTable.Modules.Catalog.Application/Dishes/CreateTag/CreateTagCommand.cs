using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.CreateTag;

public sealed record CreateTagCommand(string Value) : ICommand<Guid>;
