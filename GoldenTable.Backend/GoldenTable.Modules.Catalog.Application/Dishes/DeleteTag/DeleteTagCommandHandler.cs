using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.DeleteTag;

public sealed class DeleteTagCommandHandler(
    IDishTagRepository dishTagRepository,
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

        dishTagRepository.Remove(tag);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
