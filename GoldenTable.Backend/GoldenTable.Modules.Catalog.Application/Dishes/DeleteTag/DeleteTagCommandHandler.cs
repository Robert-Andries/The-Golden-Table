using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Dishes.Abstractions;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.DeleteTag;

public sealed class DeleteTagCommandHandler(
    IDishTagRepository dishTagRepository,
    IDishDbSets dishDbSets,
    IDishCacheService dishCacheService,
    IUnitOfWork unitOfWork,
    ILogger<DeleteTagCommandHandler> logger)
    : ICommandHandler<DeleteTagCommand>
{
    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        DishTag? tag = await dishTagRepository.GetByIdAsync(request.TagId, cancellationToken);
        if (tag is null)
        {
            DishLogs.TagNotFound(logger, request.TagId);
            return Result.Failure(DishTagErrors.NotFound);
        }

        List<Guid> dishIdsToInvalidate = await dishDbSets.Dishes
            .Where(d => d.Tags.Any(t => t.Id == request.TagId))
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        dishTagRepository.Remove(tag);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (Guid dishId in dishIdsToInvalidate)
        {
            await dishCacheService.RemoveAsync(dishId, cancellationToken);
        }

        return Result.Success();
    }
}
