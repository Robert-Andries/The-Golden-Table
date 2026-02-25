using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images.Delete;

public sealed partial class DeleteImageCommandHandler(
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    IImageCacheService imageCacheService,
    ILogger<DeleteImageCommandHandler> logger) 
    : ICommandHandler<DeleteCommand>
{
    public async Task<Result> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Image? image = await imageRepository.GetAsync(request.ImageId, cancellationToken);
        if (image is null)
        {
            ImagesLogs.ImageNotFound(logger, request.ImageId);
            return ImageErrors.NotFound;
        }

        imageRepository.Remove(image);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await imageCacheService.DeleteAsync(request.ImageId, cancellationToken);

        return Result.Success();
    }


}
