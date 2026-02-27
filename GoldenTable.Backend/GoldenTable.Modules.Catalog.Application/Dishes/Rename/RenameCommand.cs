using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;

namespace GoldenTable.Modules.Catalog.Application.Dishes.Rename;

public sealed record RenameCommand(Guid DishId, Name Name) : ICommand;
