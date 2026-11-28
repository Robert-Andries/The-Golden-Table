namespace GoldenTable.Modules.Orders.Presentation.Authentication;

public static class Permissions
{
    public static readonly string CreateOrder = "orders:create";
    public static readonly string GetOrderById = "orders:get-by-id";
    public static readonly string ViewOwnOrders = "orders:view-own-orders";
    public static readonly string GetAllOrders = "orders:get-all";
    public static readonly string ModifyItems = "orders:modify-items";
    public static readonly string RequestCancel = "orders:request-cancel";
    public static readonly string UpdateAddress = "orders:update-address";
    public static readonly string ChangeStatus = "orders:change-status";
}
