using GoldenTable.Common.Application.Messaging;

namespace GoldenTable.Modules.Catalog.Application.Images.GetAll;

public sealed record GetAllImagesQuery() : IQuery<List<ImageResponse>>;
