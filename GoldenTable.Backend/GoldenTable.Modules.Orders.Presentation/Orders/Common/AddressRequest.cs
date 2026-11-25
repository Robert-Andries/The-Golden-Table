using GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

namespace GoldenTable.Modules.Orders.Presentation.Orders.Common;

internal sealed record AddressRequest(string County, string City, string Street, string StreetNumber, string ZipCode)
{
    /// <returns>The domain address, or null when the client sent none (the domain rejects it as invalid)</returns>
    internal static Address? ToAddress(AddressRequest? request)
    {
        return request is null
            ? null
            : new Address(request.County, request.City, request.Street, request.StreetNumber, request.ZipCode);
    }
}
