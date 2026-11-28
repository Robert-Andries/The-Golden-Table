using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Orders.Domain.Dishes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenTable.Modules.Orders.Infrastructure.Dishes;

internal sealed class DishConfiguration : IEntityTypeConfiguration<Dish>
{
    public void Configure(EntityTypeBuilder<Dish> builder)
    {
        builder.HasKey(d => d.Id);

        // The id comes from the Catalog module, never from the database
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.ComplexProperty(d => d.Name, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value).IsRequired();
        });

        builder.ComplexProperty(d => d.BasePrice, basePriceBuilder =>
        {
            basePriceBuilder.Property(m => m.Amount).IsRequired();
            basePriceBuilder.Property(m => m.Currency)
                .HasConversion(c => c.Code, code => Currency.FromCode(code).Value)
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.OwnsMany(d => d.Sizes, sizesBuilder => sizesBuilder.ToJson());
        builder.Navigation(d => d.Sizes)
            .HasField("_sizes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
