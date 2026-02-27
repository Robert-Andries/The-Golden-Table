using GoldenTable.Common.Application.Messaging;
using GoldenTable.Common.Domain;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Application.Dishes.GetDishesByName;

public sealed class GetDishesByNameQueryHandler(IDishDbSets dishDbSets)
    : IQueryHandler<GetDishesByNameQuery, List<DishResponse>>
{
    public async Task<Result<List<DishResponse>>> Handle(GetDishesByNameQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<Dish> dishes = await dishDbSets.Dishes
            .AsNoTracking()
            .Where(d => d.Name.Value.Contains(request.Name.Value))
            .Include(d => d.Images)
            .Include(d => d.Tags)
            .ToListAsync(cancellationToken);

        var output = dishes.Select(d => new DishResponse(d)).ToList();

        return Result.Success(output);
    }
}
