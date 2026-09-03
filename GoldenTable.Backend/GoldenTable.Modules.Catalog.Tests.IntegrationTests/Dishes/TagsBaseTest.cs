using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using GoldenTable.Modules.Catalog.Tests.IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Tests.IntegrationTests.Dishes;

public class TagsBaseTest : BaseIntegrationTest
{
    public TagsBaseTest(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    protected async Task PutTagInDb(DishTag tag)
    {
        context.Set<DishTag>().Add(tag);
        await context.SaveChangesAsync();
    }

    protected async Task<DishTag?> GetTagFromDb(Guid tagId)
    {
        return await context.Set<DishTag>().AsNoTracking().FirstOrDefaultAsync(t => t.Id == tagId);
    }
}
