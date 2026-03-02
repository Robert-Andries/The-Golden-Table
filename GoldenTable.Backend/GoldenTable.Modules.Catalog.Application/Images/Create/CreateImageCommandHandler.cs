using GoldenTable.Common.Application.Clock;
using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.Image.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Images.Create;

public sealed class CreateImageCommandHandler(
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork,
    IImageDbSet imageDbSet,
    IImageCacheService imageCacheService,
    ILogger<CreateImageCommandHandler> logger,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateImageCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateImageCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (imageDbSet.Images.Any(i => i.Uri == request.Uri))
        {
            return Result.Failure<Guid>(ImageErrors.UriAlreadyExists);
        }
        
        Name name = new(request.Name);
        if (!name.IsValid())
        {
            return Result.Failure<Guid>(ImageErrors.InvalidName);
        }
        if (imageDbSet.Images.Any(i => i.Name == name))
        {
            return Result.Failure<Guid>(ImageErrors.NameAlreadyExists);
        }
        Description description = request.Description is null ? new("") :  new(request.Description);

        Result<Image> imageResult = Image.Create(
            dateTimeProvider.UtcNow,
            request.Uri,
            name,
            description);
        if (imageResult.IsFailure)
        {
            ImagesLogs.CreateImageError(logger, imageResult.Error);
            return Result.Failure<Guid>(imageResult.Error);
        }

        await imageRepository.AddAsync(imageResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await imageCacheService.UpdateAsync(imageResult.Value, cancellationToken);

        return Result.Success(imageResult.Value.Id);
    }
}
