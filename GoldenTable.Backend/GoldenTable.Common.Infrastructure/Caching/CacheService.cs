using System.Text;
using GoldenTable.Common.Application.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GoldenTable.Common.Infrastructure.Caching;

internal sealed class CacheService(IDistributedCache cache, ILogger<CacheService> logger) : ICacheService
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        TypeNameHandling = TypeNameHandling.All,
        ContractResolver = new PrivateSetterContractResolver()
    };

    private sealed class PrivateSetterContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(
            System.Reflection.MemberInfo member, 
            MemberSerialization memberSerialization)
        {
            Newtonsoft.Json.Serialization.JsonProperty prop = base.CreateProperty(member, memberSerialization);
            if (!prop.Writable)
            {
                var property = member as System.Reflection.PropertyInfo;
                if (property != null)
                {
                    bool hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;

                    if (!prop.Writable && prop.PropertyName != null)
                    {
                        string fieldName = $"_{char.ToLowerInvariant(prop.PropertyName[0])}{prop.PropertyName.Substring(1)}";
#pragma warning disable S3011
                        System.Reflection.FieldInfo? field = member.DeclaringType?.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
#pragma warning restore S3011
                        if (field != null)
                        {
                            prop.ValueProvider = new Newtonsoft.Json.Serialization.ReflectionValueProvider(field);
                            prop.Writable = true;
                        }
                    }
                }
            }
            return prop;
        }
    }


    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            byte[]? bytes = await cache.GetAsync(key, cancellationToken);

            return bytes is null ? default : Deserialize<T>(bytes);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get cache key '{CacheKey}', error message: '{Exception}'. Treating as cache miss.", key, ex.Message);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            byte[] bytes = Serialize(value);

            await cache.SetAsync(key, bytes, CacheOptions.Create(expiration), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to set cache key '{CacheKey}'. Skipping cache write.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to remove cache key '{CacheKey}'. Skipping cache removal.", key);
        }
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
