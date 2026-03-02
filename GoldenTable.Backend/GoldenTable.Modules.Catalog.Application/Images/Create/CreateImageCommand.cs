using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Application.Images.Create;

public sealed record CreateImageCommand(Uri Uri, string Name, string? Description) : ICommand<Guid>;
