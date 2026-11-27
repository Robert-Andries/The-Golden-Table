using GoldenTable.Common.Domain;

namespace GoldenTable.Modules.Orders.Domain.Dishes;

public static class DishErrors
{
    public static Error InvalidName { get; } =
        new("DishErrors.InvalidName", "The provided name is invalid.", ErrorType.Validation);

    public static Error SameName { get; } = new("DishErrors.SameName",
        "The provided name is same as the old one.", ErrorType.Validation);

    public static Error SamePrice { get; } = new("DishErrors.SamePrice",
        "The provided price is same as the old one.", ErrorType.Validation);

    public static Error InvalidSize { get; } = new("DishErrors.InvalidSize",
        "The provided size is invalid.", ErrorType.Validation);

    public static Error SizeAlreadyPresent { get; } = new("DishErrors.SizeAlreadyPresent",
        "The dish already have the provided size.", ErrorType.Validation);

    public static Error NotFound { get; } = new("DishErrors.NotFound",
        "The dish was not found.", ErrorType.NotFound);
}
