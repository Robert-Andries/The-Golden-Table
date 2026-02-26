using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images.GetImageById;

public sealed class GetImageByIdQueryHandler(
    IImageRepository imageRepository,
    IImageCacheService imageCacheService,
    ILogger<GetImageByIdQueryHandler> logger) 
    : IQueryHandler<GetImageByIdQuery, ImageResponse>
{
    public async Task<Result<ImageResponse>> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Image? image = await imageCacheService.GetAsync(request.ImageId, cancellationToken) ??
                       await imageRepository.GetAsync(request.ImageId, cancellationToken);
        if (image is null)
        {
            ImagesLogs.ImageNotFound(logger, request.ImageId);
            return Result.Failure<ImageResponse>(ImageErrors.NotFound);
        }
        ImageResponse output = new(image);
        
        return Result.Success(output);
    }


}
