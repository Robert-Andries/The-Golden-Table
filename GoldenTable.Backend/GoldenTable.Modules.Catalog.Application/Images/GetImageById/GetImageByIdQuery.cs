using GoldenTable.Common.Application.Messaging;
using GoldenTable.Modules.Catalog.Domain.Common.Image;

namespace GoldenTable.Modules.Catalog.Application.Images.GetImageById;

public sealed record GetImageByIdQuery(Guid ImageId) : IQuery<ImageResponse>;
