using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images.GetImageById;

public sealed partial class GetImageByIdQueryHandler(
    IImageRepository imageRepository,
    IImageCacheService imageCacheService,
    ILogger<GetImageByIdQueryHandler> logger) 
    : IQueryHandler<GetImageByIdQuery, Image>
{
    public async Task<Result<Image>> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Image? image = await imageCacheService.GetAsync(request.ImageId, cancellationToken) ??
                       await imageRepository.GetAsync(request.ImageId, cancellationToken);
        if (image is null)
        {
            ImagesLogs.ImageNotFound(logger, request.ImageId);
            return Result.Failure<Image>(ImageErrors.NotFound);
        }
        
        return Result.Success(image);
    }


}
