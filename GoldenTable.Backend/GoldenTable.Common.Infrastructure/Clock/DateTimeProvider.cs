using GoldenTable.Common.Application.Clock;

namespace GoldenTable.Common.Infrastructure.Clock;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
