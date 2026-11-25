using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Orders.Domain.Orders;

public static class OrderErrors
{
    public static Error InvalidItems { get; } =
        new("OrderErrors.InvalidItems", "The provided items are invalid.", ErrorType.Validation);

    public static Error InvalidAddress { get; } =
        new("OrderErrors.InvalidAddress", "The provided address are invalid.", ErrorType.Validation);

    public static Error SameAddress { get; } =
        new("OrderErrors.SameAddress", "The new address is the same as the old one.", ErrorType.Validation);

    public static Error InvalidStatus { get; } =
        new("OrderErrors.InvalidStatus", "You cannot change status to the provided one.", ErrorType.Validation);

    public static Error CancelAlreadyRequested { get; } =
        new("OrderErrors.CancelAlreadyRequested", "A cancel request was already requested.", ErrorType.Validation);

    public static Error CannotUpdateAddress { get; } =
        new("OrderErrors.CannotUpdateAddress", "You cannot update the address of an canceled or sent order.",
            ErrorType.Validation);

    public static Error InvalidQuantity { get; } =
        new("OrderErrors.InvalidQuantity", "The quantity should be greater then 0.", ErrorType.Validation);
    
    public static Error NotFound { get; } =
        new("OrderErrors.NotFound", "The order with the specified id was not found.", ErrorType.Validation);
}
