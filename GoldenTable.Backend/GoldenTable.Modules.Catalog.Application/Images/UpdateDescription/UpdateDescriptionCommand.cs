using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Application.Images.UpdateDescription;

public sealed record UpdateDescriptionCommand(Guid ImageId, Description Description) : ICommand;
