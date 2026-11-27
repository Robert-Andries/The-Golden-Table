using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;

namespace GoldenTable.Modules.Catalog.Application.Images.GetAll;

public sealed class GetAllImagesQueryHandler(IImageRepository imageRepository)
    : IQueryHandler<GetAllImagesQuery, List<ImageResponse>>
{
    public async Task<Result<List<ImageResponse>>> Handle(GetAllImagesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<Image> images = await imageRepository.GetAllAsync(cancellationToken);

        var output = images.Select(image => new ImageResponse(image)).ToList();

        return Result.Success(output);
    }
}
