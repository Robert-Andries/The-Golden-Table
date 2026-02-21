using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes;
using GoldenTable.Modules.Catalog.Domain.Common.ValueTypes.Money;
using GoldenTable.Modules.Catalog.Domain.Dishes;
using GoldenTable.Modules.Catalog.Domain.Dishes.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenTable.Modules.Catalog.Infrastructure.Dishes;

internal sealed class DishesConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Name)
            .IsRequired()
            .HasConversion(n => n.Value, v => new Name(v));
        builder.HasIndex(d => d.Name).IsUnique();

        builder.OwnsOne(d => d.Description, descriptionBuilder =>
        {
            descriptionBuilder.Property(d => d.Value).IsRequired();
        });

        builder.Property(d => d.Category)
            .IsRequired()
            .HasConversion(c => c.name, n => new DishCategory(n));

        builder.ComplexProperty(d => d.BasePrice, basePriceBuilder => 
        {
            basePriceBuilder.Property(m => m.Amount).IsRequired();
            basePriceBuilder.Property(m => m.Currency)
                .HasConversion(c => c.Code, code => Currency.FromCode(code).Value)
                .IsRequired()
                .HasMaxLength(3);
        });
        
        builder.ComplexProperty(d => d.NutritionalInformation, niBuilder => 
        {
            niBuilder.IsRequired();
            niBuilder.ToJson();
        });

        builder.OwnsMany(d => d.Sizes);
        builder.HasMany(d => d.Tags).WithMany();
        builder.HasMany(d => d.Images).WithOne();
    }
}
