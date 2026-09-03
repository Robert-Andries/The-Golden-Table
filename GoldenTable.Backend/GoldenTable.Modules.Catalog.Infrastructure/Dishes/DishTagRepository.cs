using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Domain.Dishes.Tag.Abstractions;
using GoldenTable.Modules.Catalog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Infrastructure.Dishes;

internal sealed class DishTagRepository(CatalogDbContext context) : IDishTagRepository
{
    public async Task<DishTag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        DishTag? tag = await context.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        return tag;
    }

    public async Task<List<DishTag>> GetByIdsAsync(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        List<DishTag> tags = await context.Tags.Where(t => ids.Contains(t.Id)).ToListAsync(cancellationToken);
        return tags;
    }

    public async Task<DishTag?> GetByValueAsync(string value, CancellationToken cancellationToken = default)
    {
        DishTag? tag = await context.Tags.FirstOrDefaultAsync(t => t.Value == value, cancellationToken);
        return tag;
    }

    public async Task<List<DishTag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<DishTag> tags = await context.Tags.ToListAsync(cancellationToken);
        return tags;
    }

    public async Task AddAsync(DishTag tag, CancellationToken cancellationToken = default)
    {
        await context.Tags.AddAsync(tag, cancellationToken);
    }

    public Task UpdateAsync(DishTag tag, CancellationToken cancellationToken = default)
    {
        context.Tags.Update(tag);
        return Task.CompletedTask;
    }

    public void Remove(DishTag tag)
    {
        context.Tags.Remove(tag);
    }
}
