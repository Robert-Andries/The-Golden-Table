using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Images.Rename;

public sealed record RenameCommand(Guid ImageId, string NewName) : ICommand;
