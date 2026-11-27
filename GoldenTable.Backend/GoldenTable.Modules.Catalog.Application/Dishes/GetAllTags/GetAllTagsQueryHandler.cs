using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetAllTags;

public sealed class GetAllTagsQueryHandler(
    IDishTagRepository dishTagRepository)
    : IQueryHandler<GetAllTagsQuery, List<TagResponse>>
{
    public async Task<Result<List<TagResponse>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<DishTag> tags = await dishTagRepository.GetAllAsync(cancellationToken);
        var output = tags.Select(tag => new TagResponse(tag)).ToList();
        
        return Result.Success(output);
    }
}
