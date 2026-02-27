using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.AddImage;

public sealed record AddImageCommand(Guid DishId, Guid ImageId) : ICommand<Guid>;
