using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.EditTag;

public sealed class EditTagCommandHandler(
    IDishTagRepository dishTagRepository,
    IDishDbSets dishDbSets,
    IDishCacheService dishCacheService,
    IUnitOfWork unitOfWork,
    ILogger<EditTagCommandHandler> logger)
    : ICommandHandler<EditTagCommand>
{
    public async Task<Result> Handle(EditTagCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        DishTag? tag = await dishTagRepository.GetByIdAsync(request.TagId, cancellationToken);
        if (tag is null)
        {
            DishLogs.TagNotFound(logger, request.TagId);
            return Result.Failure(DishTagErrors.NotFound);
        }

        DishTag? existingTag = await dishTagRepository.GetByValueAsync(request.Value, cancellationToken);
        if (existingTag is not null && existingTag.Id != request.TagId)
        {
            return Result.Failure(DishTagErrors.AlreadyExists);
        }

        Result updateResult = tag.UpdateValue(request.Value);
        if (updateResult.IsFailure)
        {
            DishLogs.EditTagError(logger, request.TagId, updateResult.Error);
            return Result.Failure(updateResult.Error);
        }

        List<Guid> dishIdsToInvalidate = await dishDbSets.Dishes
            .Where(d => d.Tags.Any(t => t.Id == request.TagId))
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        await dishTagRepository.UpdateAsync(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (Guid dishId in dishIdsToInvalidate)
        {
            await dishCacheService.RemoveAsync(dishId, cancellationToken);
        }

        return Result.Success();
    }
}
