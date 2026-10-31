using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenTable.Common.Infrastructure.Outbox;

public sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
    {
        builder.ToTable("OutboxMessageConsumers");

        builder.HasKey(c => new { c.OutboxMessageId, c.Name });

        builder.Property(c => c.Name).HasMaxLength(500);
    }
}
