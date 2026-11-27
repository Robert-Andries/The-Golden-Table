using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoldenTable.Common.Infrastructure.Inbox;

public sealed class InboxMessageConsumerConfiguration : IEntityTypeConfiguration<InboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<InboxMessageConsumer> builder)
    {
        builder.ToTable("InboxMessageConsumers");

        builder.HasKey(c => new { c.InboxMessageId, c.Name });

        builder.Property(c => c.Name).HasMaxLength(500);
    }
}
