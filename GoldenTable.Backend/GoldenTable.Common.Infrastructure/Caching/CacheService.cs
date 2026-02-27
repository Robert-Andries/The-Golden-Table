using System.Text;
using GoldenTable.Common.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace GoldenTable.Common.Infrastructure.Caching;

internal sealed class CacheService(IDistributedCache cache) : ICacheService
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        TypeNameHandling = TypeNameHandling.All
    };


    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        byte[]? bytes = await cache.GetAsync(key, cancellationToken);

        return bytes is null ? default : Deserialize<T>(bytes);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        byte[] bytes = Serialize(value);

        return cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(key, cancellationToken);
    }

    private static T Deserialize<T>(byte[] bytes)
    {
        string json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }

    private static byte[] Serialize<T>(T value)
    {
        string json = JsonConvert.SerializeObject(value, Settings);
        return Encoding.UTF8.GetBytes(json);
    }
}
