using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Images.UpdateDescription;

public sealed record UpdateDescriptionCommand(Guid ImageId, string Description) : ICommand;
