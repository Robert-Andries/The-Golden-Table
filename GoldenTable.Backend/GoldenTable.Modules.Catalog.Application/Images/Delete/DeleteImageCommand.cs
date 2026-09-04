using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Images.Delete;

public sealed record DeleteImageCommand(Guid ImageId) : ICommand;
