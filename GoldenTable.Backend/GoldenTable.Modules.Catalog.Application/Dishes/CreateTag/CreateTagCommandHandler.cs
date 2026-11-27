using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;
using Microsoft.Extensions.Logging;

namespace GoldenTable.Modules.Catalog.Application.Dishes.CreateTag;

public sealed class CreateTagCommandHandler(
    IDishTagRepository dishTagRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateTagCommandHandler> logger)
    : ICommandHandler<CreateTagCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        DishTag? existingTag = await dishTagRepository.GetByValueAsync(request.Value, cancellationToken);
        if (existingTag is not null)
        {
            return Result.Failure<Guid>(DishTagErrors.AlreadyExists);
        }

        Result<DishTag> tagResult = DishTag.Create(request.Value);
        if (tagResult.IsFailure)
        {
            DishLogs.CreateTagError(logger, tagResult.Error);
            return Result.Failure<Guid>(tagResult.Error);
        }

        await dishTagRepository.AddAsync(tagResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(tagResult.Value.Id);
    }
}
