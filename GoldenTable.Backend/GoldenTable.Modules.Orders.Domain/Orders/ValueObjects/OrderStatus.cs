namespace GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

public enum OrderStatus
{
    Placed,
    Rejected,
    Confirmed,
    Cancelled,
    Preparing,
    Sent
}
