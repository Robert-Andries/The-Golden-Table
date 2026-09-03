using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Images.EditImage;

public sealed record EditImageCommand(Guid ImageId, string Name, string? Description, Uri Uri) : ICommand;
