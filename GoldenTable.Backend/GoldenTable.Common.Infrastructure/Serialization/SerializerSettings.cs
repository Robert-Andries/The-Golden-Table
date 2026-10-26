using Newtonsoft.Json;

namespace GoldenTable.Common.Infrastructure.Serialization;

internal static class SerializerSettings
{
    internal static readonly JsonSerializerSettings Instance = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };
}
