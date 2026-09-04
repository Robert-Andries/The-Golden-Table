using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.DeleteTag;

public sealed record DeleteTagCommand(Guid TagId) : ICommand;
