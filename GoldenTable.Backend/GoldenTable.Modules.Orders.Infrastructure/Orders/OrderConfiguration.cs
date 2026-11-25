using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Orders.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenTable.Modules.Orders.Infrastructure.Orders;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        // Customers list their own orders, so this is the common lookup
        builder.HasIndex(o => o.CustomerId);

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.ComplexProperty(o => o.TotalPrice, priceBuilder =>
        {
            priceBuilder.Property(m => m.Amount).IsRequired();
            priceBuilder.Property(m => m.Currency)
                .HasConversion(c => c.Code, code => Currency.FromCode(code).Value)
                .IsRequired()
                .HasMaxLength(3);
        });

        builder.ComplexProperty(o => o.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.County).IsRequired();
            addressBuilder.Property(a => a.City).IsRequired();
            addressBuilder.Property(a => a.Street).IsRequired();
            addressBuilder.Property(a => a.StreetNumber).IsRequired();
            addressBuilder.Property(a => a.ZipCode).IsRequired();
        });

        // Items are always read and replaced together with their order, so they live in a JSON column
        builder.OwnsMany(o => o.Items, itemsBuilder => itemsBuilder.ToJson());
        builder.Navigation(o => o.Items)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
