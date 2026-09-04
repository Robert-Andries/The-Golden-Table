using GoldenTable.Modules.Catalog.Domain.Dishes.Tag;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenTable.Modules.Catalog.Infrastructure.Dishes;

internal sealed class DishTagConfiguration : IEntityTypeConfiguration<DishTag>
{
    public void Configure(EntityTypeBuilder<DishTag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Value).IsRequired();
        builder.HasIndex(t => t.Value).IsUnique();
        builder.ToTable("DishTag");
    }
}
