namespace GoldenTable.Modules.Order.Domain.Orders.ValueObjects;

public enum OrderStatus
{
    Placed,
    Rejected,
    Confirmed,
    Cancelled,
    Preparing,
    Sent,
}
