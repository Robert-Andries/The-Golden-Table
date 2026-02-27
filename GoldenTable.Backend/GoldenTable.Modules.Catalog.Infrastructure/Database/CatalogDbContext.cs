using GoldenTable.Modules.Catalog.Application.Abstractions.Data;
using GoldenTable.Modules.Catalog.Application.Abstractions.Dataset;
using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Infrastructure.Dishes;
using GoldenTable.Modules.Catalog.Infrastructure.Images;
using Microsoft.EntityFrameworkCore;

namespace GoldenTable.Modules.Catalog.Infrastructure.Database;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : DbContext(options), IUnitOfWork, IDishDbSets
{
    public DbSet<Image> Images { get; set; }
    public DbSet<Dish> Dishes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Catalog);
        modelBuilder.ApplyConfiguration(new DishesConfiguration());
        modelBuilder.ApplyConfiguration(new ImagesConfiguration());
        modelBuilder.Entity<Dish>().Ignore(d => d.DomainEvents);
        modelBuilder.Entity<Image>().Ignore(i => i.DomainEvents);
    }
}
