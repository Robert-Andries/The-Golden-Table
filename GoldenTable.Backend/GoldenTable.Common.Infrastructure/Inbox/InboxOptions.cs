namespace GoldenTable.Common.Infrastructure.Inbox;

public sealed class InboxOptions
{
    public int IntervalInSeconds { get; set; }
    public int BatchSize { get; set; }
}
