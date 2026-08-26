using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Images.Create;

public sealed record CreateImageCommand(Uri Uri, string Name, string? Description) : ICommand<Guid>;
