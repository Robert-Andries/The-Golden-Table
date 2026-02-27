using GoldenTable.Modules.Catalog.Domain.Common.Image;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenTable.Modules.Catalog.Infrastructure.Images;

internal sealed class ImagesConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name)
            .HasConversion(n => n.Value, value => new Name(value))
            .IsRequired();
        builder.HasIndex(i => i.Name).IsUnique();
        builder.OwnsOne(i => i.Description, descriptionBuilder =>
        {
            descriptionBuilder.Property(d => d.Value).IsRequired();
        });
    }
}
