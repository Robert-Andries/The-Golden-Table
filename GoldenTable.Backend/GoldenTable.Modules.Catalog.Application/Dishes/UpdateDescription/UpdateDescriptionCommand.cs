using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Application.Dishes.UpdateDescription;

public sealed record UpdateDescriptionCommand(Guid DishId, Description Description) : ICommand;
