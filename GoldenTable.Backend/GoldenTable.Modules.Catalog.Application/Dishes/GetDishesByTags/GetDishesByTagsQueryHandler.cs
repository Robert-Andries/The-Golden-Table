using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByTags;

public class GetDishesByTagsQueryHandler(IDishDbSets dishDbSets)
    : IQueryHandler<GetDishesByTagsQuery, List<DishResponse>>
{
    public async Task<Result<List<DishResponse>>> Handle(GetDishesByTagsQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<Dish> dishes = await dishDbSets.Dishes
            .AsNoTracking()
            .Include(d => d.Images)
            .Include(d => d.Tags)
            .Where(d => d.Tags.Count(t => request.TagIds.Contains(t.Id)) == request.TagIds.Count)
            .ToListAsync(cancellationToken);

        if (dishes.Count == 0)
        {
            return Result.Failure<List<DishResponse>>(DishErrors.NotFound);
        }

        return dishes.Select(d => new DishResponse(d)).ToList();
    }
}
