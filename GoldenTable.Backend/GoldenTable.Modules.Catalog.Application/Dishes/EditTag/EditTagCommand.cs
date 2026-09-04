using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.EditTag;

public sealed record EditTagCommand(Guid TagId, string Value) : ICommand;
