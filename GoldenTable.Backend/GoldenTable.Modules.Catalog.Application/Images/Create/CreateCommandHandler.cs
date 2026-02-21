using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images.Create;

public sealed partial class CreateCommandHandler(
    IImageRepository  imageRepository,
    IUnitOfWork unitOfWork,
    IImageCacheService imageCacheService,
    ILogger<CreateCommandHandler> logger,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateCommand>
{
    public async Task<Result> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        Result<Image> imageResult = Image.Create(
            dateTimeProvider.UtcNow,
            request.Uri,
            request.Name,
            request.Description);
        if (imageResult.IsFailure)
        {
            ImagesLogs.CreateImageError(logger, imageResult.Error);
            return imageResult.Error;
        }

        await imageRepository.AddAsync(imageResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await imageCacheService.UpdateAsync(imageResult.Value, cancellationToken);

        return Result.Success();
    }


}
