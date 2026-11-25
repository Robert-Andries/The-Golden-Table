namespace GoldenTable.Modules.Orders.Domain.Orders.ValueObjects;

public record Address(string County, string City, string Street, string StreetNumber, string ZipCode)
{
    public static bool IsValid(Address address)
    {
        return !string.IsNullOrWhiteSpace(address.County) &&
               !string.IsNullOrWhiteSpace(address.City) &&
               !string.IsNullOrWhiteSpace(address.Street) &&
               !string.IsNullOrWhiteSpace(address.StreetNumber) &&
               !string.IsNullOrWhiteSpace(address.ZipCode);
    }
}
