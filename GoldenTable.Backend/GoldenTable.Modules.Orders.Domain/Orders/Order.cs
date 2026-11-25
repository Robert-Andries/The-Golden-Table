using System.Collections.Frozen;
using GoldenTable.Common.Domain;
using GoldenTable.Common.Domain.Money;
using GoldenTable.Modules.Orders.Domain.Orders.Events;
using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Domain.Orders;

public sealed class Order : Entity
{
    private static readonly FrozenDictionary<OrderStatus, FrozenSet<OrderStatus>> AllowedTransitions =
        new Dictionary<OrderStatus, FrozenSet<OrderStatus>>
        {
            [OrderStatus.Placed] =
                new[] { OrderStatus.Confirmed, OrderStatus.Rejected, OrderStatus.Cancelled }.ToFrozenSet(),
            [OrderStatus.Confirmed] = new[] { OrderStatus.Preparing, OrderStatus.Cancelled }.ToFrozenSet(),
            [OrderStatus.Preparing] = new[] { OrderStatus.Sent, OrderStatus.Cancelled }.ToFrozenSet(),
            [OrderStatus.Sent] = FrozenSet<OrderStatus>.Empty,
            [OrderStatus.Rejected] = FrozenSet<OrderStatus>.Empty,
            [OrderStatus.Cancelled] = FrozenSet<OrderStatus>.Empty
        }.ToFrozenDictionary();

    private List<DishItem> _items = new();

    private Order()
    {
    }

    public OrderStatus Status { get; private set; }
    public Guid CustomerId { get; init; }
    public Money TotalPrice { get; private set; }
    public IReadOnlyCollection<DishItem> Items => _items.AsReadOnly();
    public Address Address { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime ModifiedOnUtc { get; private set; }
    public bool CancelRequested { get; private set; }

    /// <summary>
    ///     Factory method to create an order
    /// </summary>
    /// <param name="customerId">The customer id who placed the order</param>
    /// <param name="totalPrice">Total price of the order. No verification will be made</param>
    /// <param name="items">The order items</param>
    /// <param name="address">Where the order should be delivered</param>
    /// <param name="nowUtc">When was the dish created</param>
    /// <returns> Result indicating failure or success and the order object</returns>
    public static Result<Order> Create(Guid customerId, Money totalPrice, List<DishItem> items,
        Address address, DateTime nowUtc)
    {
        if (items.Count == 0 || items.Any(dishItem => dishItem.Quantity <= 0))
        {
            return Result.Failure<Order>(OrderErrors.InvalidItems);
        }

        if (!Address.IsValid(address))
        {
            return Result.Failure<Order>(OrderErrors.InvalidAddress);
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = OrderStatus.Placed,
            CustomerId = customerId,
            TotalPrice = totalPrice,
            _items = items,
            Address = address,
            CreatedOnUtc = nowUtc,
            ModifiedOnUtc = nowUtc,
            CancelRequested = false
        };

        order.Raise(new OrderCreatedDomainEvent(order.Id));

        return Result.Success(order);
    }

    /// <summary>
    ///     Updates the order address
    /// </summary>
    /// <param name="newAddress">The new address</param>
    /// <param name="nowUtc">When the action occured</param>
    /// <returns>Result indicating success or failure</returns>
    public Result UpdateAddress(Address newAddress, DateTime nowUtc)
    {
        if (Status is OrderStatus.Sent or OrderStatus.Cancelled or OrderStatus.Rejected)
        {
            return Result.Failure(OrderErrors.CannotUpdateAddress);
        }

        if (!Address.IsValid(newAddress))
        {
            return Result.Failure(OrderErrors.InvalidAddress);
        }

        if (newAddress == Address)
        {
            return Result.Failure(OrderErrors.SameAddress);
        }

        Address = newAddress;
        ModifiedOnUtc = nowUtc;

        Raise(new OrderAddressChangedDomainEvent(Id, newAddress));

        return Result.Success();
    }

    /// <summary>
    ///     Changes the status of the order
    /// </summary>
    /// <param name="newStatus">The new status</param>
    /// <param name="nowUtc">When the action occured</param>
    /// <returns>Result indicating success or failure</returns>
    public Result UpdateStatus(OrderStatus newStatus, DateTime nowUtc)
    {
        if (newStatus == Status)
        {
            return Result.Success();
        }

        if (!AllowedTransitions.TryGetValue(Status, out FrozenSet<OrderStatus> allowed) || !allowed.Contains(newStatus))
        {
            return Result.Failure(OrderErrors.InvalidStatus);
        }

        Status = newStatus;
        ModifiedOnUtc = nowUtc;

        Raise(new OrderStatusChangedDomainEvent(Id, newStatus));

        return Result.Success();
    }

    /// <summary>
    ///     Request the order to be canceled. This action is typically done by the customer.
    /// </summary>
    /// <param name="nowUtc">When this action occured</param>
    /// <returns>Result indicating success</returns>
    public Result RequestCancel(DateTime nowUtc)
    {
        if (CancelRequested)
        {
            return Result.Failure(OrderErrors.CancelAlreadyRequested);
        }

        CancelRequested = true;
        ModifiedOnUtc = nowUtc;
        Raise(new OrderCancelRequestedDomainEvent(Id));
        return Result.Success();
    }

    /// <summary>
    ///     Change the items from the order. The old items will be deleted.
    /// </summary>
    /// <param name="items">The new items</param>
    /// <param name="totalPrice">The new total price</param>
    /// <param name="nowUtc">When this action occured</param>
    /// <returns>Result indicating success or failure</returns>
    public Result ModifyItems(List<DishItem> items, Money totalPrice, DateTime nowUtc)
    {
        if (items.Count == 0 || items.Any(dishItem => dishItem.Quantity <= 0))
        {
            return Result.Failure<Order>(OrderErrors.InvalidItems);
        }

        _items = items;
        TotalPrice = totalPrice;
        ModifiedOnUtc = nowUtc;
        Raise(new OrderItemsChangedDomainEvent(Id, _items));
        return Result.Success();
    }
}
