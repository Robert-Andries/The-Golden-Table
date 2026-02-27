using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Application.Images.Rename;

public sealed record RenameCommand(Guid ImageId, Name NewName) : ICommand;
