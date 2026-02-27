using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Images.GetImageById;

public sealed record GetImageByIdQuery(Guid ImageId) : IQuery<ImageResponse>;
